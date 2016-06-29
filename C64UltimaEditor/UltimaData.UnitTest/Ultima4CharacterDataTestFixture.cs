﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UltimaData.UnitTest
{
    [TestClass]
    public class Ultima4CharacterDataTestFixture
    {
        [TestMethod]
        public void Ctor()
        {
            Ultima4CharacterData character = new Ultima4CharacterData();

            Assert.AreEqual("", character.Name);
            Assert.AreEqual(U4Sex.Female, character.Sex);
            Assert.AreEqual(U4Class.Mage, character.Class);
            Assert.AreEqual(U4Health.Good, character.Health);
            Assert.AreEqual(0, character.HitPoints);
            Assert.AreEqual(100, character.MaxHitPoints);
            Assert.AreEqual(0, character.Experience);
            Assert.AreEqual(0, character.Strength);
            Assert.AreEqual(0, character.Dexterity);
            Assert.AreEqual(0, character.Intelligence);
            Assert.AreEqual(0, character.MagicPoints);
            Assert.AreEqual(U4EquipedWeapon.Hands, character.Weapon);
            Assert.AreEqual(U4EquipedArmor.Skin, character.Armor);
        }
    }
}