using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impacter : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IImpactable impactable = collision.gameObject.GetComponent<IImpactable>();

        if (impactable == null)
            return;

        Vector2 point = collision.GetContact(0).point;
        Vector2 normal = collision.GetContact(0).normal;
        float impulse = collision.GetContact(0).normalImpulse;   /// use this to calculate what effect to apply (displacement, destruction, etc.)

        // TODO : Think about the speed loss, since velocity is manually calculated.
        // Also, friction? Or just bumping? Bumping might be annoying when navigating tight spots or docking.

        Debug.DrawLine(point, point + normal, Color.green, 10f);
        Debug.DrawLine(collision.gameObject.transform.position, collision.gameObject.transform.position + collision.gameObject.transform.up, Color.red, 10f);
        Debug.Log("BOOOOOM : " + impulse);

        impactable.Impact(normal * -impulse);
    }
}
