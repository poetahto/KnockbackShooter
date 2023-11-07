﻿using UnityEngine;

namespace FreeForAll
{
    public abstract class KnockoutStrategy : MonoBehaviour
    {
        public KnockoutManager Manager { get; set; }
        public virtual void Initialize() {}
        public virtual void OnLogic() {}
    }
}