namespace Isolation.Scripts.Equips {

    public class Visomatic: Grabbable {

        // private protected override void OnGrab() {
        //     transform.localScale = 0.75f * Vector3.one;
        //     transform.localPosition = new Vector3(0f, 0.1f, -0.1f);
        // }

        private protected override void OnShow() {
            gameObject.SetActive(true);
        }

        private protected override void OnHide() {
            gameObject.SetActive(false);
        }

    }

}