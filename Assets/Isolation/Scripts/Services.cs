#nullable enable
using System;
using UnityEngine;

namespace Isolation.Scripts {

    public static class Services {

        public static bool Production {
            get {
                var val = Environment.GetEnvironmentVariable("isolation-test");
                return val is not null && !bool.Parse(val);
            }
        }

        public static INetworkService Network = new NetworkService();

        public static IAuthService Auth = new SteamAuthService();

        public static IUnityService Unity = new UnityService();

        static Services() {
            Debug.Log("[Services] Init.");
        }

    }

}