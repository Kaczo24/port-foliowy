using System.Collections.Generic;
using UnityEngine;

public class WeaponeScript : MonoBehaviour
{
    List<PlayerControler> collided = new List<PlayerControler>();        //lista graczy, którą można zmieniać

    void OnTriggerEnter2D(Collider2D collision) // Wykonuje się, jak wejdzie do pola "trigger" broni
    {
        if (collision.tag == "Player")      //czy to "coś" oznaczyłem jako gracz?
        {
            PlayerControler pc = collision.GetComponent<PlayerControler>();     //szukamy skryptu gracza i oznaczamy go jako pc
            if (!collided.Contains(pc))                 // jeżeli jeszcze tego nie ma na liście, dodaj to
                collided.Add(pc);
        }
    }
    void OnTriggerExit2D(Collider2D collision)  // Wykonuje się, jak coś wychodzi z pola "trigger" broni
    {
        if (collision.tag == "Player")      //czy to "coś" oznaczyłem jako gracz?
            collided.Remove(collision.GetComponent<PlayerControler>()); //usuń to z listy
    }


    ///  To wszystko sprawia, że w liście "collided", znajdują się skrypty graczy, którzy są w polu "trigger" broni


    public void Damage(int amount)
    {
        foreach (PlayerControler controler in collided)
            controler.hp -= amount;
        // wszystkim graczom w w polu "trigger" broni, odojmij hp
    }
}
