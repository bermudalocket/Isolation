using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Isolation.Scripts {

    public enum Loot {
        Cash, UpgradeChips
    }
    
    [RequireComponent(typeof(CursorModifier))]
    public class Lootable: MonoBehaviour {

        private bool _isLooted = false;

        public void OnClick() {
            if (_isLooted) {
                return;
            }
            var loots = GenerateLoot();
            foreach (var loot in loots) {
                switch (loot) {
                    case Loot.Cash:
                        PlayerState.Cash += (int) RandomGaussian.Range(10, 20);
                        break;
                    
                    case Loot.UpgradeChips:
                        PlayerState.UpgradeChips += (int) RandomGaussian.Range(1, 3);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            UniTask.Void(DoEffect);
            _isLooted = true;
        }

        private async UniTaskVoid DoEffect() {
            await Addressables.InstantiateAsync(
                "Assets/Standard Assets/Sherbbs Particle Collection/Particles/Normal/BirthdaySpark.prefab",
                transform.position, 
                Quaternion.identity);
            var clip = await Addressables.LoadAssetAsync<AudioClip>("Assets/Isolation/Sound/Loot.wav");
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        private static IEnumerable<Loot> GenerateLoot() {
            var lootTypes = Enum.GetValues(typeof(Loot));
            var random = new System.Random();
            var output = new Loot[UnityEngine.Random.Range(1, 3)];
            for (var i = 0; i < output.Length; i++) {
                output[i] = (Loot) lootTypes.GetValue(random.Next(lootTypes.Length));
            }
            return output;
        } 

    }

}