#nullable enable
using System;
using Cysharp.Threading.Tasks;
using Steamworks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Isolation.Scripts {
    
    public interface IAuthService {
        
        public string? Username { get; }

        public UniTask<Result<bool>> SignIn();

    }

    public sealed class AnonAuthService: IAuthService {

        public string? Username {
            get {
                if (!AuthenticationService.Instance.IsSignedIn) {
                    return null;
                }
                return AuthenticationService.Instance.PlayerId;
            }
        }

        public async UniTask<Result<bool>> SignIn() {
            if (UnityServices.State == ServicesInitializationState.Uninitialized) {
                await UnityServices.InitializeAsync();
            }
            if (AuthenticationService.Instance.IsSignedIn) {
                Debug.Log("Already signed in.");
                return new Success<bool>(true);
            }
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            return new Success<bool>(true);
        }

    }

    public sealed class SteamAuthService: IAuthService {

        public string? Username {
            get {
                if (!SteamAPI.IsSteamRunning()) return null;
                try {
                    return SteamFriends.GetPersonaName();
                } catch {
                    return null;
                }
            }
        }

        public async UniTask<Result<bool>> SignIn() {
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn) {
                Debug.Log("Already signed in.");
                return new Success<bool>(true);
            }
            if (!SteamAPI.IsSteamRunning()) {
                return new Failure<bool>("Steam is not running.");
            }
            SteamAPI.Init();
            var buffer = new byte[1024];
            SteamUser.GetAuthSessionTicket(buffer, buffer.Length, out var ticketSize);
            Array.Resize(ref buffer, (int) ticketSize);
            var sessionTicket = BitConverter.ToString(buffer).Replace("-", string.Empty);
            try {
                await AuthenticationService.Instance.SignInWithSteamAsync(sessionTicket);
                return new Success<bool>(true);
            } catch (Exception e) {
                return new Failure<bool>("Error signing in: " + e);
            }
        }

    }

}