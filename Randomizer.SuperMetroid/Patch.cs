﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using static Randomizer.SuperMetroid.ItemType;
using System.Text.RegularExpressions;

namespace Randomizer.SuperMetroid {

    class Patch {

        readonly List<World> allWorlds;
        readonly World myWorld;
        readonly string seedGuid;
        readonly int seed;
        readonly Random rnd;
        Dictionary<int, byte[]> patches;

        #region "Save the animals" patch data
        static readonly Dictionary<string, byte[]> animalPatches = new Dictionary<string, byte[]> {
            ["Animals as Enemies"] = new byte [] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x07, 0x84, 0x18, 0x00, 0x06, 0x3B, 0xB6,
                0x00, 0x00, 0x1C, 0x8C, 0x07, 0x98, 0x61, 0x00, 0x07, 0xBB, 0x91, 0x12,
                0x84, 0x05, 0xB9, 0xBB, 0x10, 0x72, 0xD7, 0x00, 0x04, 0x01, 0x00, 0x58,
                0x02, 0x10, 0x72, 0xFB, 0x00, 0x02, 0x00, 0x00, 0x10, 0x73, 0x03, 0x00,
                0x03, 0x23, 0x80, 0x2D, 0x10, 0x73, 0x0F, 0x00, 0x0C, 0xC6, 0xEE, 0x00,
                0x00, 0x00, 0x0C, 0x44, 0xE9, 0x01, 0x00, 0x58, 0x02, 0x10, 0x73, 0x3B,
                0x00, 0x02, 0x00, 0x00, 0x10, 0x73, 0x43, 0x00, 0x03, 0x23, 0x80, 0x2D,
                0x10, 0x73, 0x4F, 0x00, 0x02, 0xC6, 0xEE, 0x10, 0x8E, 0xD5, 0x00, 0x08,
                0xDF, 0x00, 0xBA, 0x00, 0x00, 0x00, 0x00, 0x20, 0x19, 0xE6, 0xE6, 0x00,
                0x01, 0xA0, 0x21, 0xE0, 0xDB, 0x00, 0x63, 0x00, 0x87, 0xD9, 0x06, 0x03,
                0xB3, 0x83, 0xB2, 0x83, 0x59, 0xFF, 0x00, 0x03, 0xB3, 0x83, 0xB3, 0x8B,
                0xDC, 0x20, 0x00, 0x8B, 0x43, 0x92, 0x83, 0xD9, 0x40, 0x05, 0x92, 0x83,
                0x40, 0x94, 0x0C, 0xC4, 0xD9, 0x60, 0x05, 0xB2, 0x83, 0x60, 0x94, 0x2C,
                0xD4, 0xDB, 0x80, 0x03, 0x60, 0x9C, 0x2C, 0xDC, 0xDB, 0x80, 0x02, 0x40,
                0x9C, 0x0C, 0xDA, 0x20, 0x00, 0xB2, 0x44, 0x83, 0x92, 0xDB, 0xA0, 0xF0,
                0x3F, 0x62, 0x00, 0x00, 0xAF, 0x5C, 0x83, 0xB0, 0x01, 0xB1, 0x83, 0xE4,
                0x3F, 0x80, 0xE4, 0x60, 0x00, 0x00, 0x41, 0x2E, 0x00, 0x00, 0xFF, 0x2E,
                0x00, 0x00, 0xFE, 0x2E, 0x00, 0x00, 0xFD, 0xE4, 0x6D, 0x00, 0x21, 0xE1,
                0x3F, 0x00, 0x00, 0x00, 0x2E, 0xFF, 0x45, 0x4F, 0x46
            },
            ["Animals has grey door"] = new byte [] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x07, 0x98, 0xD8, 0x00, 0x02, 0x00, 0xFC,
                0x07, 0xFC, 0x00, 0x00, 0x0E, 0x44, 0xDB, 0x08, 0x08, 0x10, 0x00, 0x42,
                0xC8, 0x2E, 0x06, 0xEE, 0x90, 0x00, 0x00, 0x07, 0xFD, 0x30, 0x00, 0x08,
                0xE3, 0xEE, 0x1D, 0x07, 0x00, 0x00, 0x00, 0x00, 0x21, 0xE1, 0x86, 0x00,
                0x01, 0x06, 0x21, 0xE1, 0x8E, 0x00, 0x8F, 0xF8, 0x59, 0x60, 0x02, 0x8B,
                0xB2, 0x83, 0xF8, 0x5B, 0xC0, 0x07, 0x92, 0x83, 0x90, 0x83, 0x40, 0x94,
                0x0C, 0xC4, 0xF8, 0x57, 0xC0, 0x07, 0x0C, 0xC0, 0x40, 0x90, 0x60, 0x94,
                0x2C, 0xD4, 0xF8, 0x57, 0xC0, 0x07, 0x2C, 0xD0, 0x60, 0x90, 0x60, 0x9C,
                0x2C, 0xDC, 0xF8, 0x58, 0x60, 0x05, 0xD8, 0x60, 0x98, 0x40, 0x9C, 0x0C,
                0xF8, 0x58, 0x60, 0x03, 0x0C, 0xD8, 0x40, 0x98, 0x83, 0x22, 0x01, 0xF0,
                0x53, 0x22, 0x01, 0x83, 0x3E, 0x02, 0xC3, 0x04, 0xC5, 0x5E, 0xF8, 0x51,
                0x62, 0x83, 0x7E, 0x01, 0xC3, 0x04, 0xC7, 0xC0, 0xF8, 0x4F, 0xC4, 0x83,
                0xDE, 0x01, 0xC3, 0x04, 0x03, 0xAF, 0x82, 0xAD, 0x82, 0xF8, 0x5B, 0x04,
                0xE8, 0xBF, 0x44, 0x80, 0xE5, 0x20, 0x00, 0x00, 0x41, 0xE4, 0x2B, 0x00,
                0x03, 0x40, 0x01, 0x00, 0xFF, 0xE4, 0x2B, 0x00, 0x03, 0xFF, 0x01, 0x00,
                0xFE, 0xE4, 0x2B, 0x00, 0x03, 0xFE, 0x01, 0x00, 0xFD, 0xE4, 0x2B, 0x00,
                0x01, 0xFD, 0x01, 0xE5, 0x1F, 0x00, 0x21, 0xE2, 0x1D, 0x00, 0x00, 0x00,
                0x14, 0xFF, 0x45, 0x4F, 0x46
            },
            ["Animals have to be killed"] = new byte [] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x07, 0x84, 0x1D, 0x00, 0x01, 0x8C, 0x07,
                0x98, 0x61, 0x00, 0x07, 0xBB, 0x91, 0x12, 0x84, 0x05, 0xB9, 0xBB, 0x10,
                0x72, 0xD7, 0x00, 0x02, 0x01, 0x00, 0x10, 0x72, 0xFB, 0x00, 0x02, 0x00,
                0x00, 0x10, 0x73, 0x05, 0x00, 0x01, 0x2D, 0x10, 0x73, 0x17, 0x00, 0x02,
                0x01, 0x00, 0x10, 0x73, 0x3B, 0x00, 0x02, 0x00, 0x00, 0x10, 0x73, 0x45,
                0x00, 0x01, 0x2D, 0x10, 0x8E, 0xDC, 0x00, 0x01, 0x20, 0x19, 0xE6, 0xE6,
                0x00, 0x01, 0xA0, 0x21, 0xE0, 0xDB, 0x00, 0x63, 0x00, 0x87, 0xD9, 0x06,
                0x03, 0xB3, 0x83, 0xB2, 0x83, 0x59, 0xFF, 0x00, 0x03, 0xB3, 0x83, 0xB3,
                0x8B, 0xDC, 0x20, 0x00, 0x8B, 0x43, 0x92, 0x83, 0xD9, 0x40, 0x05, 0x92,
                0x83, 0x40, 0x94, 0x0C, 0xC4, 0xD9, 0x60, 0x05, 0xB2, 0x83, 0x60, 0x94,
                0x2C, 0xD4, 0xDB, 0x80, 0x03, 0x60, 0x9C, 0x2C, 0xDC, 0xDB, 0x80, 0x02,
                0x40, 0x9C, 0x0C, 0xDA, 0x20, 0x00, 0xB2, 0x44, 0x83, 0x92, 0xDB, 0xA0,
                0xF0, 0x3F, 0x62, 0x00, 0x00, 0xAF, 0x5C, 0x83, 0xB0, 0x01, 0xB1, 0x83,
                0xE4, 0x3F, 0x80, 0xE4, 0x60, 0x00, 0x00, 0x41, 0x2E, 0x00, 0x00, 0xFF,
                0x2E, 0x00, 0x00, 0xFE, 0x2E, 0x00, 0x00, 0xFD, 0xE4, 0x6D, 0x00, 0x21,
                0xE1, 0x3F, 0x00, 0x00, 0x00, 0x2E, 0xFF, 0x45, 0x4F, 0x46
            },
            ["Animals is Draygon"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0xAD, 0xA0, 0x00, 0x24, 0xFD, 0x92,
                0x00, 0x05, 0x3E, 0x26, 0x03, 0x02, 0x00, 0x80, 0xA2, 0xB9, 0x60, 0xDA,
                0x40, 0x04, 0x01, 0x16, 0x00, 0x01, 0x00, 0x80, 0x00, 0x00, 0x79, 0x98,
                0x40, 0x05, 0x2E, 0x06, 0x02, 0x00, 0x00, 0x80, 0x00, 0x00, 0x07, 0x98,
                0xDC, 0x00, 0x02, 0x06, 0xF0, 0x07, 0xF0, 0x00, 0x00, 0x04, 0xA0, 0xAD,
                0xAC, 0xAD, 0x07, 0xF0, 0x06, 0x00, 0x12, 0xA9, 0x00, 0x00, 0x8F, 0x2C,
                0xD8, 0x7E, 0xA9, 0x00, 0xF0, 0x8F, 0xB5, 0x07, 0x7E, 0x5C, 0xBB, 0x91,
                0x8F, 0x07, 0xDA, 0x8A, 0x00, 0x02, 0x18, 0xF0, 0x07, 0xEF, 0xF0, 0x00,
                0x02, 0xB8, 0xAD, 0x07, 0xF0, 0x18, 0x00, 0x53, 0xAF, 0x20, 0xD8, 0x7E,
                0x89, 0x00, 0x40, 0xF0, 0x46, 0xA9, 0xF0, 0xEF, 0x8F, 0xB5, 0x07, 0x7E,
                0xA9, 0xFF, 0xFF, 0x8F, 0xD5, 0x1C, 0x7E, 0xA9, 0xAE, 0x80, 0x8F, 0xBE,
                0x01, 0x7F, 0xA9, 0xCE, 0x80, 0x8F, 0xFE, 0x01, 0x7F, 0xA9, 0xCE, 0x88,
                0x8F, 0x3E, 0x02, 0x7F, 0xA9, 0xAE, 0x88, 0x8F, 0x7E, 0x02, 0x7F, 0x08,
                0xE2, 0x20, 0xA9, 0x00, 0x8F, 0xC2, 0x66, 0x7F, 0x8F, 0xE2, 0x66, 0x7F,
                0x8F, 0x02, 0x67, 0x7F, 0x8F, 0x22, 0x67, 0x7F, 0xC2, 0x20, 0x28, 0x5C,
                0xDD, 0xC8, 0x8F, 0x5C, 0xDD, 0xC8, 0x8F, 0x45, 0x4F, 0x46
            },
            ["Animals is Ridley"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0xAD, 0xA0, 0x00, 0x24, 0xFD, 0x92,
                0x00, 0x05, 0x3E, 0x26, 0x03, 0x02, 0x00, 0x80, 0xA2, 0xB9, 0x2E, 0xB3,
                0x40, 0x04, 0x01, 0x16, 0x00, 0x01, 0x00, 0x80, 0x00, 0x00, 0x79, 0x98,
                0x40, 0x05, 0x2E, 0x06, 0x02, 0x00, 0x00, 0x80, 0x00, 0x00, 0x07, 0x98,
                0xDC, 0x00, 0x02, 0x06, 0xF0, 0x07, 0xF0, 0x00, 0x00, 0x04, 0xA0, 0xAD,
                0xAC, 0xAD, 0x07, 0xF0, 0x06, 0x00, 0x12, 0xA9, 0x00, 0x00, 0x8F, 0x2A,
                0xD8, 0x7E, 0xA9, 0x00, 0xF0, 0x8F, 0xB5, 0x07, 0x7E, 0x5C, 0xBB, 0x91,
                0x8F, 0x07, 0xB3, 0x58, 0x00, 0x02, 0x18, 0xF0, 0x07, 0xEF, 0xF0, 0x00,
                0x02, 0xB8, 0xAD, 0x07, 0xF0, 0x18, 0x00, 0x42, 0xAF, 0x20, 0xD8, 0x7E,
                0x89, 0x00, 0x40, 0xF0, 0x35, 0xA9, 0x00, 0x00, 0x8F, 0xBB, 0xD8, 0x7E,
                0xA9, 0xF0, 0xEF, 0x8F, 0xB5, 0x07, 0x7E, 0xA9, 0xAA, 0xAA, 0x8F, 0xD5,
                0x1C, 0x7E, 0xA9, 0xAE, 0x80, 0x8F, 0xDE, 0x00, 0x7F, 0xA9, 0xCE, 0x80,
                0x8F, 0xFE, 0x00, 0x7F, 0xA9, 0xCE, 0x88, 0x8F, 0x1E, 0x01, 0x7F, 0xA9,
                0xAE, 0x88, 0x8F, 0x3E, 0x01, 0x7F, 0x5C, 0xF7, 0x91, 0x8F, 0x5C, 0xF7,
                0x91, 0x8F, 0x45, 0x4F, 0x46
            },
            ["Animals is Phantoon"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0xAD, 0xA0, 0x00, 0x24, 0xFD, 0x92,
                0x00, 0x05, 0x3E, 0x26, 0x03, 0x02, 0x00, 0x80, 0xA2, 0xB9, 0x13, 0xCD,
                0x40, 0x04, 0x01, 0x06, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x79, 0x98,
                0x40, 0x05, 0x2E, 0x06, 0x02, 0x00, 0x00, 0x80, 0x00, 0x00, 0x07, 0x98,
                0xDC, 0x00, 0x02, 0x06, 0xF0, 0x07, 0xF0, 0x00, 0x00, 0x04, 0xA0, 0xAD,
                0xAC, 0xAD, 0x07, 0xF0, 0x06, 0x00, 0x12, 0xA9, 0x00, 0x00, 0x8F, 0x2B,
                0xD8, 0x7E, 0xA9, 0x00, 0xF0, 0x8F, 0xB5, 0x07, 0x7E, 0x5C, 0xBB, 0x91,
                0x8F, 0x07, 0xCD, 0x3D, 0x00, 0x02, 0x18, 0xF0, 0x07, 0xEF, 0xF0, 0x00,
                0x02, 0xB8, 0xAD, 0x07, 0xF0, 0x18, 0x00, 0x1F, 0xAF, 0x20, 0xD8, 0x7E,
                0x89, 0x00, 0x40, 0xF0, 0x12, 0xA9, 0x00, 0x00, 0x8F, 0xC0, 0xD8, 0x7E,
                0xA9, 0xF0, 0xEF, 0x8F, 0xB5, 0x07, 0x7E, 0x5C, 0xD0, 0xC8, 0x8F, 0x5C,
                0xD0, 0xC8, 0x8F, 0x45, 0x4F, 0x46
            },
            ["Animals is Metal Pirates"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0xAD, 0xA0, 0x00, 0x24, 0xFD, 0x92,
                0x00, 0x05, 0x3E, 0x26, 0x03, 0x02, 0x00, 0x80, 0xA2, 0xB9, 0x2B, 0xB6,
                0x40, 0x04, 0x01, 0x06, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x79, 0x98,
                0x40, 0x05, 0x2E, 0x06, 0x02, 0x00, 0x00, 0x80, 0x00, 0x00, 0x07, 0x98,
                0xDC, 0x00, 0x02, 0x06, 0xF0, 0x07, 0xF0, 0x00, 0x00, 0x04, 0xA0, 0xAD,
                0xAC, 0xAD, 0x07, 0xF0, 0x06, 0x00, 0x0B, 0xA9, 0x00, 0xF0, 0x8F, 0xB5,
                0x07, 0x7E, 0x5C, 0xBB, 0x91, 0x8F, 0x07, 0xB6, 0x50, 0x00, 0x02, 0x18,
                0xF0, 0x07, 0xEF, 0xF0, 0x00, 0x04, 0xB8, 0xAD, 0xAC, 0xAD, 0x07, 0xF0,
                0x18, 0x00, 0x1C, 0xAF, 0x20, 0xD8, 0x7E, 0x89, 0x00, 0x40, 0xF0, 0x12,
                0xA9, 0x00, 0x00, 0x8F, 0xBC, 0xD8, 0x7E, 0xA9, 0xF0, 0xEF, 0x8F, 0xB5,
                0x07, 0x7E, 0x5C, 0xF7, 0x91, 0x8F, 0x60, 0x45, 0x4F, 0x46
            },
            ["Animals is back to escape"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0xAD, 0xA0, 0x00, 0x18, 0xFD, 0x92,
                0x00, 0x05, 0x3E, 0x26, 0x03, 0x02, 0x00, 0x80, 0xA2, 0xB9, 0x4D, 0xDE,
                0x40, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x80, 0x00, 0x00, 0x07, 0x98,
                0xDC, 0x00, 0x02, 0x06, 0xF0, 0x07, 0xF0, 0x00, 0x00, 0x04, 0xA0, 0xAD,
                0xAC, 0xAD, 0x07, 0xF0, 0x06, 0x00, 0x0B, 0xA9, 0x00, 0xF0, 0x8F, 0xB5,
                0x07, 0x7E, 0x5C, 0xBB, 0x91, 0x8F, 0x45, 0x4F, 0x46
            },
            ["Animals ends game"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0x8B, 0xCC, 0x00, 0x02, 0x20, 0xFF,
                0x07, 0xFF, 0x20, 0x00, 0x11, 0xAF, 0x20, 0xD8, 0x7E, 0x89, 0x00, 0x40,
                0xF0, 0x07, 0xA9, 0x26, 0x00, 0x8F, 0x98, 0x09, 0x7E, 0x60, 0x45, 0x4F,
                0x46
            },
            ["Animals sets the timer low"] = new byte[] {
                0x50, 0x41, 0x54, 0x43, 0x48, 0x01, 0x8B, 0xCC, 0x00, 0x02, 0x20, 0xFF,
                0x07, 0xFF, 0x20, 0x00, 0x11, 0xAF, 0x20, 0xD8, 0x7E, 0x89, 0x00, 0x40,
                0xF0, 0x07, 0xA9, 0x20, 0x00, 0x8F, 0x46, 0x09, 0x7E, 0x60, 0x45, 0x4F,
                0x46
            }
        };
  
        #endregion 

        public Patch(World myWorld, List<World> allWorlds, string seedGuid, Random rnd) {
            this.myWorld = myWorld;
            this.allWorlds = allWorlds;
            this.seedGuid = seedGuid;
            this.rnd = rnd;
        }

        
        private Dictionary<int, byte[]> ApplyIps(byte[] ipsData) {
            var patches = new Dictionary<int, byte[]>();
            using var br = new BinaryReader(new MemoryStream(ipsData));

            var patch = new string(br.ReadChars(5));
            if (patch != "PATCH") {
                throw new InvalidDataException("The argument data is not an IPS patch.");
            }

            var chunkHeader = br.ReadBytes(3);
            while (!(chunkHeader[0] == 'E' && chunkHeader[1] == 'O' && chunkHeader[2] == 'F')) {
                var offset = BeInt(chunkHeader);
                var length = BeInt(br.ReadBytes(2));
                
                if(length > 0) {
                    patches.Add(offset, br.ReadBytes(length));
                } else {
                    var runLength = BeInt(br.ReadBytes(2));
                    byte runByte = br.ReadByte();
                    patches.Add(offset, Enumerable.Repeat(runByte, runLength).ToArray());
                }

                chunkHeader = br.ReadBytes(3);
            }

            return patches;
        }

        private byte[] GetSMItemPLM(Location location) {
            int plmId = myWorld.Config.GameMode == GameMode.Multiworld ?
                0xEFE0 :
                location.Item.Type switch
                {
                    ETank => 0xEED7,
                    Missile => 0xEEDB,
                    Super => 0xEEDF,
                    PowerBomb => 0xEEE3,
                    Bombs => 0xEEE7,
                    Charge => 0xEEEB,
                    Ice => 0xEEEF,
                    HiJump => 0xEEF3,
                    SpeedBooster => 0xEEF7,
                    Wave => 0xEEFB,
                    Spazer => 0xEEFF,
                    SpringBall => 0xEF03,
                    Varia => 0xEF07,
                    Plasma => 0xEF13,
                    Grapple => 0xEF17,
                    Morph => 0xEF23,
                    ReserveTank => 0xEF27,
                    Gravity => 0xEF0B,
                    XRay => 0xEF0F,
                    SpaceJump => 0xEF1B,
                    ScrewAttack => 0xEF1F,
                    _ => 0xEFE0,
                };

            plmId += plmId switch
            {
                0xEFE0 => location.Type switch
                {
                    LocationType.Chozo => 4,
                    LocationType.Hidden => 8,
                    _ => 0
                },
                _ => location.Type switch
                {
                    LocationType.Chozo => 0x54,
                    LocationType.Hidden => 0xA8,
                    _ => 0
                }
            };

            return BitConverter.GetBytes((ushort)plmId);
        }

        public Dictionary<int, byte[]> Create() {
            patches = new Dictionary<int, byte[]>();

            WriteLocations();
            WritePlayerNames();
            WriteSeedData();
            WriteSeedHash();
            WriteItemLocations();
            WriteAnimalSurprise();

            return patches;
        }

        void WriteLocations() {
            foreach(var location in myWorld.Locations) {
                /* Write the correct PLM to the item location */
                patches.Add(location.Address, GetSMItemPLM(location));

                if (myWorld.Config.GameMode == GameMode.Multiworld) {
                    /* Write item information to new randomizer item table */
                    var type = location.Item.World == location.Region.World ? 0 : 1;
                    var itemId = (byte)location.Item.Type;
                    var owner = location.Item.World.Id;

                    patches.Add(0x1C6000 + (location.Id * 8), new[] { type, itemId, owner, 0 }.SelectMany(UshortBytes).ToArray());
                }
            }
        }

        void WriteAnimalSurprise() {
            patches = patches.Concat(ApplyIps(animalPatches.Values.ElementAt(rnd.Next(animalPatches.Count - 1)))).ToDictionary(p => p.Key, p => p.Value);
        }

        void WritePlayerNames() {
            foreach (var world in allWorlds) {
                patches.Add(0x1C5000 + (world.Id * 16), PlayerNameBytes(world.Player));
            }
        }
        
        byte[] PlayerNameBytes(string name) {
            name = name.Length > 12 ? name[..12] : name;
            int padding = 12 - name.Length;
            if (padding > 0) {
                double pad = padding / 2.0;
                name = name.PadLeft(name.Length + (int)Math.Ceiling(pad));
                name = name.PadRight(name.Length + (int)Math.Floor(pad));
            }
            return AsAscii(name.ToUpper()).Concat(UintBytes(0)).ToArray();
        }

        void WriteSeedData() {
            var configField =
                ((myWorld.Config.GameMode == GameMode.Multiworld ? 1 : 0) << 12) |
                /* Gap of 2 bits, taken by Z3 logic in combo */
                ((int)myWorld.Config.Logic << 8) |
                (Randomizer.version.Major << 4) |
                (Randomizer.version.Minor << 0);

            patches.Add(0x1C4F00, UshortBytes(myWorld.Id));
            patches.Add(0x1C4F02, UshortBytes(configField));
            patches.Add(0x1C4F04, UintBytes(seed));
            /* Reserve the rest of the space for future use */
            patches.Add(0x1C4F08, Enumerable.Repeat<byte>(0x00, 8).ToArray());
            patches.Add(0x1C4F10, AsAscii(seedGuid));
            patches.Add(0x1C4F30, AsAscii(myWorld.Guid));

            if (myWorld.Config.GameMode == GameMode.Multiworld) {
                /* Write multiworld config flag into the ROM */
                patches.Add(0x277F00, BitConverter.GetBytes((ushort)1));
            }
        }

        void WriteSeedHash() {
            var seedHash = new byte[4];
            rnd.NextBytes(seedHash);
            patches.Add(0x2FFF00, seedHash);
        }

        void WriteItemLocations() {
            int romAddress = 0x2F5240;
            foreach (var location in myWorld.Locations.OrderBy(l => l.Region.Name).ThenBy(l => l.Name).Where(l => l.Item.Class == ItemClass.Major)) {
                patches.Add(romAddress, AsCreditsString(0x04, location.Item.Name, true));
                patches.Add(romAddress + 0x40, AsCreditsString(0x18, location.Name, false));
                romAddress += 0x80;
            }

            patches.Add(romAddress, new byte[] { 0, 0, 0, 0 });
        }

        byte[] UintBytes(int value) => BitConverter.GetBytes((uint)value);

        byte[] UshortBytes(int value) => BitConverter.GetBytes((ushort)value);

        byte[] AsAscii(string text) => Encoding.ASCII.GetBytes(text);

        int BeInt(byte[] bytes) => bytes.Select((x, i) => (x, i)).Aggregate(0, (t, n) => t | (n.x << (8 * (bytes.Length - n.i - 1))));

        byte[] AsCreditsString(int color, string text, bool alignLeft) {
            var creditsText = Regex.Replace(text.ToUpper(), "[^A-Z0-9\\.,'!: ]+", "");
            if (alignLeft) {
                creditsText = " " + creditsText[..Math.Min(creditsText.Length, 30)].PadRight(31, ' ');
            } else {
                creditsText = " " + creditsText[..Math.Min(creditsText.Length, 30)].PadLeft(30, '.') + " ";
            }
            
            return creditsText.Select(c => new byte[] {
                c switch { ' ' => 0x7f, '!' => 0x1f, ':' => 0x1e, '\'' => 0x1d, '_' => 0x1c, ',' => 0x1b, '.' => 0x1a, _ => (byte)(c - 0x41) },
                (byte)(c == ' ' ? 0x00 : color)
            }).SelectMany(x => x).ToArray();
        }
    }
}
