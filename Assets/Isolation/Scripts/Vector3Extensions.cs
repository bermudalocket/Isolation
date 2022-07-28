using System.Collections.Generic;
using UnityEngine;

namespace Isolation.Scripts {

    public static class Vector3Extensions {

        public static string Description(this List<Vector3> list) {
            var str = "";
            foreach (var thing in list) {
                if (thing == Vector3.forward) {
                    str += "forward, ";
                } else if (thing == Vector3.back) {
                    str += "back, ";
                } else if (thing == Vector3.right) {
                    str += "right, ";
                } else {
                    str += "left, ";
                }
            }
            return str;
        }

    }

}