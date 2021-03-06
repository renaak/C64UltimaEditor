﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaData
{
    public enum U2Sex
    {
        Female = 0xC6,
        Male = 0xCD
    }

    public enum U2Class
    {
        Fighter = 0x00,
        Cleric = 0x01,
        Wizard = 0x02,
        Thief = 0x03
    }

    public enum U2Race
    {
        Human = 0x00,
        Elf = 0x01,
        Dwarf = 0x02,
        Hobbit = 0x03
    }

    public enum U2Weapons
    {
        Dagger,
        Mace,
        Axe,
        Bow,
        Sword,
        GreatSword,
        LightSword,
        Phaser,
        QuickBlade
    }

    public enum U2Armor
    {
        Cloth,
        Leather,
        Chain,
        Plate,
        Reflect,
        Power
    }

    public enum U2Spells
    {
        Light,
        LadderDown,
        LadderUp,
        Passwall,
        Surface,
        Prayer,
        MagicMissle,
        Blink,
        Kill
    }

    public enum U2Items
    {
        Rings,
        Wands,
        Staffs,
        Boots,
        Cloaks,
        Helms,
        Gems,
        Ankhs,
        RedGems,
        SkullKeys,
        GreenGems,
        BrassButtons,
        BlueTassles,
        StrangeCoins,
        GreenIdols,
        TryLithiums
    }

    public enum U2Map
    {
        TimeOfLegends,
        Pangea,
        Medieval,
        Modern,
        Aftermath
    }

    public class U2Location
    {
        public U2Location()
        {
            Map = U2Map.TimeOfLegends;
            m_X = new BoundedInt(0, 255);
            X = 0;
            m_Y = new BoundedInt(0, 255);
            Y = 0;
        }
        public U2Map Map;

        public int X
        {
            get { return m_X; }
            set { m_X.Value = value; }
        }
        private BoundedInt m_X;

        public int Y
        {
            get { return m_Y; }
            set { m_Y.Value = value; }
        }
        private BoundedInt m_Y;
    }

    public class Ultima2Data
    {
        public Ultima2Data(IFile file = null)
        {
            m_name = "";

            Sex = U2Sex.Female;
            Class = U2Class.Fighter;
            Race = U2Race.Human;

            m_hitPoints = new BoundedInt(0, 9999);
            HitPoints = 0;

            m_experience = new BoundedInt(0, 9999);
            Experience = 0;

            m_strength = new BoundedInt(0, 99);
            Strength = 0;
            m_agility = new BoundedInt(0, 99);
            Agility = 0;
            m_stamina = new BoundedInt(0, 99);
            Stamina = 0;
            m_charisma = new BoundedInt(0, 99);
            Charisma = 0;
            m_wisdom = new BoundedInt(0, 99);
            Wisdom = 0;
            m_intelligence = new BoundedInt(0, 99);
            Intelligence = 0;

            Spells = new BoundedIntArray(9, 0, 99);
            Armor = new BoundedIntArray(6, 0, 99);
            Weapons = new BoundedIntArray(9, 0, 99);

            m_food = new BoundedInt(0, 9999);
            Food = 0;
            m_gold = new BoundedInt(0, 9999);
            Gold = 0;

            m_torches = new BoundedInt(0, 99);
            Torches = 0;
            m_keys = new BoundedInt(0, 99);
            Keys = 0;
            m_tools = new BoundedInt(0, 99);
            Tools = 0;

            Items = new BoundedIntArray(16, 0, 99);

            Location = new U2Location();

            RawFile = null;

            if (file == null)
                File = new File();
            else
                File = file;
        }

        public void Save(string file)
        {
            if (RawFile == null)
                throw new InvalidOperationException("Cannot save a file without loading one first.");

            SaveName();

            RawFile[SexOffset] = (byte)Sex;
            RawFile[ClassOffset] = (byte)Class;
            RawFile[RaceOffset] = (byte)Race;

            RawFile[HitPointsOffset] = ConvertIntToBCD(HitPoints / 100);
            RawFile[HitPointsOffset + 1] = ConvertIntToBCD(HitPoints % 100);

            RawFile[ExperienceOffset] = ConvertIntToBCD(Experience / 100);
            RawFile[ExperienceOffset + 1] = ConvertIntToBCD(Experience % 100);

            RawFile[StrengthOffset] = ConvertIntToBCD(Strength);
            RawFile[AgilityOffset] = ConvertIntToBCD(Agility);
            RawFile[StaminaOffset] = ConvertIntToBCD(Stamina);
            RawFile[CharismaOffset] = ConvertIntToBCD(Charisma);
            RawFile[WisdomOffset] = ConvertIntToBCD(Wisdom);
            RawFile[IntelligenceOffset] = ConvertIntToBCD(Intelligence);

            for (int i = 0; i < Spells.Length; ++i)
                RawFile[SpellsOffset + i] = ConvertIntToBCD(Spells[i]);

            for (int i = 0; i < Armor.Length; ++i)
                RawFile[ArmorOffset + i] = ConvertIntToBCD(Armor[i]);

            for (int i = 0; i < Weapons.Length; ++i)
                RawFile[WeaponsOffset + i] = ConvertIntToBCD(Weapons[i]);

            RawFile[FoodOffset] = ConvertIntToBCD(Food / 100);
            RawFile[FoodOffset + 1] = ConvertIntToBCD(Food % 100);

            RawFile[GoldOffset] = ConvertIntToBCD(Gold / 100);
            RawFile[GoldOffset + 1] = ConvertIntToBCD(Gold % 100);

            RawFile[TorchesOffset] = ConvertIntToBCD(Torches);
            RawFile[KeysOffset] = ConvertIntToBCD(Keys);
            RawFile[ToolsOffset] = ConvertIntToBCD(Tools);

            for (int i = 0; i < Items.Length; ++i)
                RawFile[ItemsOffset + i] = ConvertIntToBCD(Items[i]);

            RawFile[MapOffset] = (byte)Location.Map;
            RawFile[LocationXOffset] = (byte)Location.X;
            RawFile[LocationYOffset] = (byte)Location.Y;

            File.WriteAllBytes(file, RawFile);
        }

        public void Load(string file)
        {
            RawFile = File.ReadAllBytes(file);

            if (!IsU2PlayerDisk())
            {
                throw new FileNotFoundException("This does not appear to be an Ultima 2 Player disk image.");
            }

            m_name = ProcessName();

            Sex = (U2Sex)RawFile[SexOffset];
            Class = (U2Class)RawFile[ClassOffset];
            Race = (U2Race)RawFile[RaceOffset];

            HitPoints = ConvertBCDToInt(RawFile[HitPointsOffset]) * 100 + ConvertBCDToInt(RawFile[HitPointsOffset + 1]);
            Experience = ConvertBCDToInt(RawFile[ExperienceOffset]) * 100 + ConvertBCDToInt(RawFile[ExperienceOffset + 1]);

            Strength = ConvertBCDToInt(RawFile[StrengthOffset]);
            Agility = ConvertBCDToInt(RawFile[AgilityOffset]);
            Stamina = ConvertBCDToInt(RawFile[StaminaOffset]);
            Charisma = ConvertBCDToInt(RawFile[CharismaOffset]);
            Wisdom = ConvertBCDToInt(RawFile[WisdomOffset]);
            Intelligence = ConvertBCDToInt(RawFile[IntelligenceOffset]);

            for (int i = 0; i < 9; ++i)
                Spells[i] = ConvertBCDToInt(RawFile[SpellsOffset + i]);

            for (int i = 0; i < 6; ++i)
                Armor[i] = ConvertBCDToInt(RawFile[ArmorOffset + i]);

            for (int i = 0; i < 9; ++i)
                Weapons[i] = ConvertBCDToInt(RawFile[WeaponsOffset + i]);

            Food = ConvertBCDToInt(RawFile[FoodOffset]) * 100 + ConvertBCDToInt(RawFile[FoodOffset + 1]);
            Gold = ConvertBCDToInt(RawFile[GoldOffset]) * 100 + ConvertBCDToInt(RawFile[GoldOffset + 1]);

            Torches = ConvertBCDToInt(RawFile[TorchesOffset]);
            Keys = ConvertBCDToInt(RawFile[KeysOffset]);
            Tools = ConvertBCDToInt(RawFile[ToolsOffset]);

            for (int i = 0; i < 16; ++i)
                Items[i] = ConvertBCDToInt(RawFile[ItemsOffset + i]);

            Location.Map = (U2Map)RawFile[MapOffset];
            Location.X = RawFile[LocationXOffset];
            Location.Y = RawFile[LocationYOffset];

        }

        private bool IsU2PlayerDisk()
        {
            if (RawFile.Length != 174848)
                return false;

            for (int i = 0; i < DiskDescriptorText.Length; ++i)
            {
                if ((byte)RawFile[DiskDescriptorOffset + i] != DiskDescriptorText[i])
                    return false;
            }

            return true;
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (value.Length < 1 || value.Length > 11)
                    throw new FormatException("Name must be between 1 and 11 characters long.");

                for (int i = 0; i < value.Length; ++i)
                    if (value[i] < 'A' || value[i] > 'Z')
                        throw new FormatException("Name must contain only uppercase letters.");

                m_name = value;
            }
        }
        private string m_name;

        public U2Sex Sex;
        public U2Class Class;
        public U2Race Race;

        public int HitPoints
        {
            get { return m_hitPoints; }
            set { m_hitPoints.Value = value; }
        }
        private BoundedInt m_hitPoints;

        public int Experience
        {
            get { return m_experience; }
            set { m_experience.Value = value; }
        }
        private BoundedInt m_experience;

        public int Strength
        {
            get { return m_strength; }
            set { m_strength.Value = value; }
        }
        private BoundedInt m_strength;

        public int Agility
        {
            get { return m_agility; }
            set { m_agility.Value = value; }
        }
        private BoundedInt m_agility;

        public int Stamina
        {
            get { return m_stamina; }
            set { m_stamina.Value = value; }
        }
        private BoundedInt m_stamina;

        public int Charisma
        {
            get { return m_charisma; }
            set { m_charisma.Value = value; }
        }
        private BoundedInt m_charisma;

        public int Wisdom
        {
            get { return m_wisdom; }
            set { m_wisdom.Value = value; }
        }
        private BoundedInt m_wisdom;

        public int Intelligence
        {
            get { return m_intelligence; }
            set { m_intelligence.Value = value; }
        }
        private BoundedInt m_intelligence;

        public readonly BoundedIntArray Spells;
        public readonly BoundedIntArray Armor;
        public readonly BoundedIntArray Weapons;

        public int Food
        {
            get { return m_food; }
            set { m_food.Value = value; }
        }
        private BoundedInt m_food;

        public int Gold
        {
            get { return m_gold; }
            set { m_gold.Value = value; }
        }
        private BoundedInt m_gold;

        public int Torches
        {
            get { return m_torches; }
            set { m_torches.Value = value; }
        }
        private BoundedInt m_torches;

        public int Keys
        {
            get { return m_keys; }
            set { m_keys.Value = value; }
        }
        private BoundedInt m_keys;

        public int Tools
        {
            get { return m_tools; }
            set { m_tools.Value = value; }
        }
        private BoundedInt m_tools;

        public BoundedIntArray Items;

        public readonly U2Location Location;

        private string ProcessName()
        {
            StringBuilder name = new StringBuilder();

            int i = 0;
            while (i < 11 && RawFile[SaveFileStartOffset + i] != 0)
            {
                name.Append((char)((RawFile[SaveFileStartOffset + i++] - 0xc1) + (byte)'A'));
            }

            return name.ToString();
        }

        private void SaveName()
        {
            for(int i = 0; i < m_name.Length; ++i)
            {
                RawFile[SaveFileStartOffset + i] = (byte)(m_name[i] - 'A' + 0xC1);
            }
        }

        private int ConvertBCDToInt(byte numberToConvert)
        {
            return (((numberToConvert & 0xf0) >> 0x04) * 10) + (numberToConvert & 0x0f);
        }

        private byte ConvertIntToBCD(int numberToConvert)
        {
            return (byte)(((numberToConvert / 10) << 0x04) | (numberToConvert % 10));
        }

        private byte[] RawFile;
        private IFile File;

        private const int DiskDescriptorOffset = 0x16590;
        private const string DiskDescriptorText = "U II-PLAYER DISK";

        private int SaveFileStartOffset = 0x2AA00;
        private int SexOffset = 0x2AA10;
        private int ClassOffset = 0x2AA11;
        private int RaceOffset = 0x2AA12;
        private int HitPointsOffset = 0x2AA1B;
        private int ExperienceOffset = 0x2AA20;
        private int StrengthOffset = 0x2AA15;
        private int AgilityOffset = 0x2AA16;
        private int StaminaOffset = 0x2AA17;
        private int CharismaOffset = 0x2AA18;
        private int WisdomOffset = 0x2AA19;
        private int IntelligenceOffset = 0x2AA1A;
        private int SpellsOffset = 0x2AA81;
        private int ArmorOffset = 0x2AA61;
        private int WeaponsOffset = 0x2AA41;
        private int GoldOffset = 0x2AA22;
        private int FoodOffset = 0x2AA1D;
        private int TorchesOffset = 0x2AA2E;
        private int KeysOffset = 0x2AA2F;
        private int ToolsOffset = 0x2AA30;
        private int ItemsOffset = 0x2AAA0;
        private int MapOffset = 0x2AA13;
        private int LocationXOffset = 0x2AA24;
        private int LocationYOffset = 0x2AA25;
    }
}
