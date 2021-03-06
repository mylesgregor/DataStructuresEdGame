﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.WorldGeneration
{
    /**
     * A Basic Block in the World
     * This class is for level file serialization.
     */
    [Serializable]
    class BlockJSON
    {
        public string type;
        public string logId; // the ID of the object when it is logged.
        public double x;
        public double y;

        public virtual string SaveString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
