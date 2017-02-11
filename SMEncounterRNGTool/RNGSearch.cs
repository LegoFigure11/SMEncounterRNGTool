﻿using System;
using System.Collections.Generic;

namespace SMEncounterRNGTool
{
    class RNGSearch
    {
        // Search Settings
        public int TSV;
        public int gender_ratio;
        public bool nogender;
        public bool AlwaysSynchro;
        public int Synchro_Stat;
        public int FrameCorrection;
        public bool Fix3v;
        public bool ShinyCharm;
        public bool Honey, UB, UB_S, Wild;
        public bool Sync;
        public int Lv_max, Lv_min;
        public int PokeLv;
        public int UB_th;
        public static List<ulong> Rand;

        private int index;


        public class RNGResult
        {
            public int Nature;
            public int Clock;
            public uint PID, EC, PSV;
            public UInt64 row_r;
            public int[] IVs;
            public int[] p_Status;
            public bool Shiny;
            public bool Synchronize;
            public int Blink;

            public int Encounter = -1;
            public int Gender;
            public int Ability = -1;
            public int UbValue = 100;
            public int Slot = -1;
            public int Lv = -1;
            public int Item = -1;
        }

        public RNGResult Generate()
        {
            RNGResult st = new RNGResult();

            index = 0;

            //シンクロ -- Synchronize
            st.row_r = currentrand();
            st.Clock = (int)(st.row_r % 17);
            st.Blink = ((int)(st.row_r & 0x7F)) > 0 ? 0 : 1;

            if (UB && Honey)
                st.UbValue = getUBValue();

            if (Sync)
                st.Synchronize = (int)(getrand() % 100) >= 50;
            if (AlwaysSynchro)
                st.Synchronize = true;

            if (Wild && !Honey)
                st.Encounter = (int)(getrand() % 100);
            else
                st.Encounter = -1;

            if (UB && !Honey)
                st.UbValue = getUBValue();

            if (UB_S || !Wild)
                st.Lv = PokeLv;

            if (Wild && !UB_S)
            {
                st.Slot = getslot((int)(getrand() % 100));
                st.Lv = (int)(getrand() % (ulong)(Lv_max - Lv_min + 1)) + Lv_min;
                st.Item = (int)(getrand() % 60);
            }

            //Something
            Advance(60 + FrameCorrection);

            //Encryption Constant
            st.EC = (uint)(getrand() & 0xFFFFFFFF);

            //PID
            int roll_count = ShinyCharm ? 3 : 1;
            if (UB_S) roll_count = 1;
            for (int i = 0; i < roll_count; i++) //pid
            {
                st.PID = (uint)(getrand() & 0xFFFFFFFF);
                st.PSV = ((st.PID >> 16) ^ (st.PID & 0xFFFF)) >> 4;
                if (st.PSV == TSV)
                {
                    st.Shiny = true;
                    break;
                }
            }

            //IV
            st.IVs = new int[6] { 0, 0, 0, 0, 0, 0 };

            int cnt = Fix3v ? 3 : 0;
            while (cnt > 0)
            {
                int ran = (int)(getrand() % 6);
                if (st.IVs[ran] != 31)
                {
                    st.IVs[ran] = 31;
                    cnt--;
                }
            }

            for (int i = 0; i < 6; i++)
                if (st.IVs[i] != 31)
                    st.IVs[i] = (int)(getrand() & 0x1F);
            
            //Ability
            if ((Wild || AlwaysSynchro) && (!UB_S))
            st.Ability = (int)(getrand() & 1) + 1;

            //Nature
            st.Nature = (int)(currentrand() % 25);
            if (st.Synchronize)
            {
                if (Synchro_Stat >= 0) st.Nature = Synchro_Stat;
            }
            else
                index++;

            //Gender
            if (nogender || UB_S)
                st.Gender = 0;
            else
                st.Gender = ((int)(getrand() % 252) >= gender_ratio) ? 1 : 2;

            return st;
        }

        private ulong getrand()
        {
            return Rand[index++];
        }

        private ulong currentrand()
        {
            return Rand[index];
        }

        private void Advance(int d)
        {
            index += d;
        }

        private int getUBValue()
        {
            int UbValue = (int)(getrand() % 100);
            Fix3v = UB_S = UbValue < UB_th;
            return UbValue;
        }

        public static int getslot(int rand)
        {
            if (rand < 20)
                return 1;
            if (rand < 40)
                return 2;
            if (rand < 50)
                return 3;
            if (rand < 60)
                return 4;
            if (rand < 70)
                return 5;
            if (rand < 80)
                return 6;
            if (rand < 90)
                return 7;
            if (rand < 95)
                return 8;
            if (rand < 99)
                return 9;
            return 10;
        }
    }
}
