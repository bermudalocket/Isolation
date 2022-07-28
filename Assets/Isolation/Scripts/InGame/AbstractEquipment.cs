using System;
using Isolation.Scripts.Equips;

namespace Isolation.Scripts.InGame {

    public abstract class AbstractEquipment: Grabbable {

        public Type Type { get; set; }
        
        public int Level { get; set; }
        
    }

}