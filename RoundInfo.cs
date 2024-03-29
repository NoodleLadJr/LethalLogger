﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalLogger
{
    public class RoundInfo
    {
        public int living;
        public int dead;
        public int quota;
        public int scrapMax;
        public int scrapReal;
        public int daysRemaining;
        public String planetName;
        public String weather;
        public String seed;
        public List<String> unlockables = new List<String>();
        public List<(String,Vector3)> randomTeleports = new List<(String,Vector3)>();
        public IDictionary<string, int> gear = new Dictionary<string, int>();
        public IDictionary<string, string> playerStatus = new Dictionary<string, string>();
    }
}
