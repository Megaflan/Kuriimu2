using Komponent.Contract.Enums;
using Komponent.IO;
using Kompression.Contract;
using Kompression.Contract.DataClasses.Encoder.Huffman;
using Kompression.Contract.DataClasses.Encoder.LempelZiv;
using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.DataClasses.Encoder.LempelZiv.MatchParser;
using Kompression.Contract.Encoder.LempelZiv.MatchFinder;
using Kompression.Contract.Enums.Encoder.Huffman;
using Kompression.Contract.Enums.Encoder.LempelZiv;
using Kompression.DataClasses.Configuration;
using Kompression.Encoder.Huffman;
using Kompression.Encoder.LempelZiv.InputManipulation;
using Kompression.Encoder.LempelZiv.MatchFinder;
using Kompression.Encoder.LempelZiv.MatchParser;
using Kompression.Extensions;
using Kompression.InternalContract.SlimeMoriMori.Decoder;
using Kompression.InternalContract.SlimeMoriMori.Deobfuscator;
using Kompression.InternalContract.SlimeMoriMori.Encoder;
using Kompression.InternalContract.SlimeMoriMori.Obfuscator;
using Kompression.InternalContract.SlimeMoriMori.ValueReader;
using Kompression.InternalContract.SlimeMoriMori.ValueWriter;
using Kompression.Specialized.SlimeMoriMori.Decoder;
using Kompression.Specialized.SlimeMoriMori.Deobfuscator;
using Kompression.Specialized.SlimeMoriMori.Encoder;
using Kompression.Specialized.SlimeMoriMori.Obfuscator;
using Kompression.Specialized.SlimeMoriMori.ValueReader;
using Kompression.Specialized.SlimeMoriMori.ValueWriter;

namespace Kompression.Specialized.SlimeMoriMori
{
    /// <summary>
    /// The <see cref="ICompression"/> for the compression in Slime Mori Mori for the GBA.
    /// </summary>
    public class SlimeMoriMoriCompression : ICompression
    {
        private readonly int _huffmanMode;
        private readonly int _compressionMode;
        private readonly int _obfuscationMode;
        private readonly bool _isCompressable;

        public string[] Names => new[] { "Slime Mori Mori" };

        public SlimeMoriMoriCompression()
        {
            _isCompressable = false;
        }

        public SlimeMoriMoriCompression(int huffmanMode, int compressionMode, int obfuscationMode)
        {
            _huffmanMode = huffmanMode;
            _compressionMode = compressionMode;
            _obfuscationMode = obfuscationMode;
            _isCompressable = true;
        }

        public void Decompress(Stream input, Stream output)
        {
            var originalInputPosition = input.Position;
            if (input.ReadByte() != 0x70)
                return;

            input.Position = 7;
            var identByte = input.ReadByte();

            var valueReader = CreateValueReader((identByte >> 3) & 0x3);
            var decoder = CreateDecoder(identByte & 0x7, valueReader);

            input.Position = originalInputPosition;
            decoder.Decode(input, output);

            var deobfuscator = CreateDeobfuscator((identByte >> 5) & 0x7);
            if (deobfuscator != null)
            {
                output.Position = 0;
                deobfuscator.Deobfuscate(output);
            }
        }

        public void Compress(Stream input, Stream output)
        {
            if (!_isCompressable)
                throw new InvalidOperationException("This instance doesn't allow for compression.");

            var inputArray = input.ToArray();

            // Obfuscate the data
            var obfuscator = CreateObfuscator(_obfuscationMode);
            if (obfuscator != null)
            {
                output.Position = 0;
                obfuscator.Obfuscate(inputArray);
                output.Position = 0;
            }

            // Find all Lz matches
            var matches = FindMatches(new MemoryStream(inputArray), _compressionMode, _huffmanMode);

            // Create huffman tree and value writer based on match filtered values
            var huffmanInput = RemoveMatchesFromInput(inputArray, matches);
            var tree = CreateHuffmanTree(huffmanInput, _huffmanMode);
            var valueWriter = CreateValueWriter(_huffmanMode, tree);

            using var bw = new BinaryBitWriter(output, BitOrder.MostSignificantBitFirst, 4, ByteOrder.LittleEndian);

            // Write header data
            bw.WriteBits((int)input.Length, 0x18);
            bw.WriteByte(0x70);

            var identByte = _compressionMode & 0x7;
            identByte |= (_huffmanMode & 0x3) << 3;
            identByte |= (_obfuscationMode & 0x7) << 5;
            bw.WriteByte(identByte);

            // Write huffman tree
            WriteHuffmanTree(bw, tree, _huffmanMode);

            // Encode the input data
            var encoder = CreateEncoder(_compressionMode, valueWriter);
            encoder.Encode(input, bw, matches);

            // Flush the buffer
            bw.Flush();
        }

        private LempelZivMatch[] FindMatches(Stream input, int compressionMode, int huffmanMode)
        {
            ILempelZivMatchFinder[] matchFinders;
            switch (compressionMode)
            {
                case 1:
                    matchFinders = new[]
                    {
                        new HistoryMatchFinder(new LempelZivMatchFinderOptions
                        {
                            Limitations = new LempelZivMatchLimitations
                            {
                                MinLength = 3,
                                MaxLength = 18,
                                MinDisplacement = 1,
                                MaxDisplacement = 0xFFFF
                            },
                            UnitSize = UnitSize.Byte
                        })
                    };
                    break;

                case 2:
                    matchFinders = new[]
                    {
                        new HistoryMatchFinder(new LempelZivMatchFinderOptions
                        {
                            Limitations = new LempelZivMatchLimitations
                            {
                                MinLength = 3,
                                MaxLength = -1,
                                MinDisplacement = 1,
                                MaxDisplacement = 0xFFFF
                            },
                            UnitSize = UnitSize.Byte
                        })
                    };
                    break;

                case 3:
                    matchFinders = new[]
                    {
                        new HistoryMatchFinder(new LempelZivMatchFinderOptions
                        {
                            Limitations = new LempelZivMatchLimitations
                            {
                                MinLength = 4,
                                MaxLength = -1,
                                MinDisplacement = 2,
                                MaxDisplacement = 0xFFFF
                            },
                            UnitSize = UnitSize.Byte
                        })
                    };
                    break;

                case 4:
                    return Array.Empty<LempelZivMatch>();

                case 5:
                    matchFinders = new ILempelZivMatchFinder[]
                    {
                        new HistoryMatchFinder(new LempelZivMatchFinderOptions
                        {
                            Limitations = new LempelZivMatchLimitations
                            {
                                MinLength = 3,
                                MaxLength = 0x42,
                                MinDisplacement = 1,
                                MaxDisplacement = 0xFFFF
                            },
                            UnitSize = UnitSize.Byte
                        }),
                        new RleMatchFinder(new LempelZivMatchFinderOptions
                        {
                            Limitations = new LempelZivMatchLimitations
                            {
                                MinLength = 1,
                                MaxLength = 0x40
                            },
                            UnitSize = UnitSize.Byte
                        })
                    };
                    break;

                default:
                    throw new InvalidOperationException($"Unknown compression mode {compressionMode}.");
            }

            // Optimal parse all LZ matches
            var parser = new OptimalLempelZivMatchParser(new LempelZivMatchParserOptions
            {
                InputManipulation = new InputManipulator(new LempelZivInputAdjustmentOptions()),
                PriceCalculator = new SlimePriceCalculator(compressionMode, huffmanMode),
                MatchFinders = matchFinders,
                UnitSize = compressionMode == 3 ? UnitSize.Short : UnitSize.Byte,
                TaskCount = 8
            });

            return parser.ParseMatches(input).ToArray();
        }

        private void WriteHuffmanTree(BinaryBitWriter bw, HuffmanTreeNode rootNode, int huffmanMode)
        {
            int bitDepth;
            switch (huffmanMode)
            {
                case 1:
                    bitDepth = 4;
                    break;
                case 2:
                    bitDepth = 8;
                    break;
                default:
                    return;
            }

            var depthList = rootNode.Children;
            for (var i = 0; i < 16; i++)
            {
                var valuesWithBitCount = depthList.Count(x => x.IsLeaf);
                bw.WriteByte(valuesWithBitCount);

                foreach (var value in depthList.Where(x => x.IsLeaf).Select(x => x.Code))
                {
                    bw.WriteBits(value, bitDepth);
                }
                depthList = depthList.Where(x => !x.IsLeaf).SelectMany(x => x.Children).ToArray();
            }
        }

        private byte[] RemoveMatchesFromInput(byte[] input, LempelZivMatch[] matches)
        {
            var huffmanInput = new byte[input.Length - matches.Sum(x => x.Length)];

            var huffmanInputPosition = 0;
            var inputArrayPosition = 0;
            foreach (var match in matches)
            {
                for (var i = inputArrayPosition; i < match.Position; i++)
                    huffmanInput[huffmanInputPosition++] = input[i];

                inputArrayPosition += match.Position - inputArrayPosition;
                inputArrayPosition += match.Length;
            }

            for (var i = inputArrayPosition; i < input.Length; i++)
                huffmanInput[huffmanInputPosition++] = input[i];

            return huffmanInput;
        }

        #region Create methods

        private IValueReader CreateValueReader(int huffmanMode)
        {
            switch (huffmanMode)
            {
                case 1:
                    return new HuffmanReader(4);

                case 2:
                    return new HuffmanReader(8);

                default:
                    return new DefaultValueReader();
            }
        }

        private ISlimeDecoder CreateDecoder(int decompMode, IValueReader huffmanReader)
        {
            switch (decompMode)
            {
                case 1:
                    return new SlimeMode1Decoder(huffmanReader);

                case 2:
                    return new SlimeMode2Decoder(huffmanReader);

                case 3:
                    return new SlimeMode3Decoder(huffmanReader);

                case 4:
                    return new SlimeMode4Decoder(huffmanReader);

                default:
                    return new SlimeMode5Decoder(huffmanReader);
            }
        }

        private ISlimeDeobfuscator CreateDeobfuscator(int deobfuscateMode)
        {
            switch (deobfuscateMode)
            {
                case 1:
                    return new SlimeMode1Deobfuscator();

                case 2:
                    return new SlimeMode2Deobfuscator();

                case 3:
                    return new SlimeMode3Deobfuscator();

                case 4:
                    return new SlimeMode4Deobfuscator();
            }

            return null;
        }

        private HuffmanTreeNode CreateHuffmanTree(byte[] input, int huffmanMode)
        {
            switch (huffmanMode)
            {
                case 1:
                    var tree = new HuffmanTreeBuilder();
                    var rootNode = tree.Build(input, 4, NibbleOrder.LowNibbleFirst);
                    SortHuffmanTree(rootNode);
                    return rootNode;

                case 2:
                    tree = new HuffmanTreeBuilder();
                    rootNode = tree.Build(input, 8, NibbleOrder.LowNibbleFirst);
                    SortHuffmanTree(rootNode);
                    return rootNode;

                default:
                    return null;
            }
        }

        private IValueWriter CreateValueWriter(int huffmanMode, HuffmanTreeNode rootNode)
        {
            switch (huffmanMode)
            {
                case 1:
                case 2:
                    return new HuffmanWriter(rootNode);
                default:
                    return new DefaultValueWriter();
            }
        }

        private void SortHuffmanTree(HuffmanTreeNode rootNode)
        {
            var treeDepth = rootNode.GetDepth();

            IList<HuffmanTreeNode> previousDepthList = new List<HuffmanTreeNode> { rootNode };
            IList<HuffmanTreeNode> depthList = rootNode.Children;
            for (int i = 0; i < treeDepth; i++)
            {
                if (depthList.All(x => !x.IsLeaf))
                {
                    previousDepthList = depthList;
                    depthList = previousDepthList.SelectMany(x => x.Children).ToList();
                    continue;
                }

                var ordered = depthList.OrderBy(x => !x.IsLeaf).ToList();
                for (var j = 0; j < ordered.Count; j++)
                {
                    previousDepthList[j / 2].Frequency -= previousDepthList[j / 2].Children[j % 2].Frequency;
                    previousDepthList[j / 2].Children[j % 2] = ordered[j];
                    previousDepthList[j / 2].Frequency += ordered[j].Frequency;
                }

                previousDepthList = ordered.Where(x => !x.IsLeaf).ToList();
                depthList = previousDepthList.SelectMany(x => x.Children).ToList();
            }
        }

        private ISlimeEncoder CreateEncoder(int compressionMode, IValueWriter valueWriter)
        {
            // TODO: Implement all encoders
            switch (compressionMode)
            {
                case 1:
                    return new SlimeMode1Encoder(valueWriter);
                case 2:
                    return new SlimeMode2Encoder(valueWriter);
                case 3:
                    return new SlimeMode3Encoder(valueWriter);
                case 4:
                    return new SlimeMode4Encoder(valueWriter);
                default:
                    return new SlimeMode5Encoder(valueWriter);
            }
        }

        private ISlimeObfuscator CreateObfuscator(int obfuscationMode)
        {
            switch (obfuscationMode)
            {
                case 1:
                    return new SlimeMode1Obfuscator();
                case 2:
                    return new SlimeMode2Obfuscator();
                case 3:
                    return new SlimeMode3Obfuscator();
                case 4:
                    return new SlimeMode4Obfuscator();
            }

            return null;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            // Nothing to dispose
        }

        #endregion
    }
}
