using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.Test.DatFiles
{
    public class ItemDictionaryTests
    {
        [Theory]
        [InlineData(ItemKey.NULL, 2)]
        [InlineData(ItemKey.Machine, 2)]
        [InlineData(ItemKey.CRC, 1)]
        [InlineData(ItemKey.SHA1, 4)]
        public void BucketByTest(ItemKey itemKey, int expected)
        {
            // Setup the items
            var rom1 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            var rom2 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            var rom3 = new Rom { Machine = new Machine { Name = "game-2" } };
            rom3.SetName("rom-3");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000ea4014ce66679e7e17d56ac510f67e39e26");
            rom3.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            var rom4 = new Rom { Machine = new Machine { Name = "game-2" } };
            rom4.SetName("rom-4");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000151d437442e74e5134023fab8bf694a2487");
            rom4.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = [rom1, rom2],
                ["game-2"] = [rom3, rom4],
            };

            dict.BucketBy(itemKey, DedupeType.None);
            Assert.Equal(expected, dict.Keys.Count);
        }
    
        [Fact]
        public void ClearEmptyTest()
        {
            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = [new Rom(),],
                ["game-2"] = [],
                ["game-3"] = null,
            };

            dict.ClearEmpty();
            Assert.Single(dict.Keys);
        }

        [Fact]
        public void ClearMarkedTest()
        {
            // Setup the items
            var rom1 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            var rom2 = new Rom { Machine = new Machine { Name = "game-1" }, Remove = true };
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = [rom1, rom2],
            };

            dict.ClearMarked();
            string key = Assert.Single(dict.Keys);
            Assert.Equal("game-1", key);
            Assert.NotNull(dict[key]);
            Assert.Single(dict[key]!);
        }
    
        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetDuplicatesTest(bool hasDuplicate, int expected)
        {
            // Setup the items
            var rom1 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            var rom2 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = [rom1, rom2],
            };

            // Setup the test item
            var rom = new Rom { Machine = new Machine { Name = "game-1" } };
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, hasDuplicate ? 1024 : 2048);

            var actual = dict.GetDuplicates(rom);
            Assert.Equal(expected, actual.Count);
        }
    
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasDuplicatesTest(bool expected)
        {
            // Setup the items
            var rom1 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            var rom2 = new Rom { Machine = new Machine { Name = "game-1" } };
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 1024);

            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = [rom1, rom2],
            };

            // Setup the test item
            var rom = new Rom { Machine = new Machine { Name = "game-1" } };
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, expected ? 1024 : 2048);

            bool actual = dict.HasDuplicates(rom);
            Assert.Equal(expected, actual);
        }
    
        [Fact]
        public void ResetStatisticsTest()
        {
            var dict = new ItemDictionary { GameCount = 1 };
            dict.ResetStatistics();
            Assert.Equal(0, dict.GameCount);
        }
    }
}