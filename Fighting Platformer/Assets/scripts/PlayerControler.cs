using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class PlayerControler : MonoBehaviour
{
    public int hp, dmg;
    public float speed, jumpForce = 20, attackDelay;        //różne zmienne, które chcesz zobaczyć w unity
    [Space] //odstęp w inspektorze unity
    public KeyCode left;
    public KeyCode right, jump, attack;  //starodawne key config
    [Space]
    public Image hpBar;
    public Text hpText;
    public GameObject weapone; // linki do innych obiektów, które będą potrzebne

    Rigidbody2D rb; // fizyka
    bool canJump = false, canAttack = true;     // testy, kiedy można wykonać różne akcje
    int maxHp;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();       //znajdujemy w "obiekcie gry" fragment odnośnie fizyki i nazywamy do rb
        maxHp = hp;
    }

    void Update()
    {
        if (Input.GetKey(right))            //czy wciska to, co ustawiliśmy jako klawisz w prowo?
        {
            weapone.transform.localPosition = new Vector3(0.65f, 0);        
            //  ^ przerzucenie broni na prawą stronę postaci. localPosition aby broń ciągle była przyklejona do postaci
            rb.velocity = new Vector2(speed, rb.velocity.y);    
            //  ^ ustawiamy poziomą prędkość w rb na prędkość (+w prawo, -w lewo), z zachowaniem starej prędkości pionowej 
        }
        else if (Input.GetKey(left))
        {
            weapone.transform.localPosition = new Vector3(-0.65f, 0);
            rb.velocity = new Vector2(-speed, rb.velocity.y);           // to samo, tylko na lewo
        }
        else
            rb.velocity = new Vector2(0, rb.velocity.y);        // jak nie wciska ani w prawo, ani w lewo, to pozioma prędkość na 0



        if (canJump && (int)rb.velocity.y == 0 && Input.GetKey(jump))     
            //czy może skoczyć, czy nie ma prędkości pionowej(bugfix) i czy wciska skok
            //int przy prędkości pionowej jest dlatego, że porównywanie floatów nie działa, a intów tak
        {
            rb.AddForce(new Vector2(0, jumpForce));     //przyspiesza do góry aby skoczyć
            canJump = false;    // teraz nie skacz więcej
        }

        if (canAttack && Input.GetKeyDown(attack))      //czy może atakować i czy wciska atak
        {
            StartCoroutine(PerformAttack());        // start nowej akcji dziejącej się w czasie a nazwie "PerformAttack"
        }


        hpBar.fillAmount = (float)hp / maxHp;   //ustawia poziom wypełniena paska życia

        if (hp < 0)
            hpText.text = "0";
        else
            hpText.text = hp.ToString();
        // ustawia tekst na pasku życia, upewniając się, że nie zejdzie poniżej 0

        if (hp <= 0)
            Destroy(gameObject);    // jak nie ma zycia, to niszczy "obiekt gry" na którym spoczywa
    }

    void OnTriggerStay2D(Collider2D collision)      // Wykonuje się, jak coś jest w polu "trigger" pod kwadratem
    {
        if (collision.tag == "wall")    //czy to "coś" oznaczyłem jako ściana?
            canJump = true;             //jak tak, to stoi na ziemi i może skakać
        if (collision.tag == "deathSpace")      //jak to coś to przystrzeń śmierci pod planszą
            hp = 0;         // to zabij
    }
    void OnTriggerExit2D(Collider2D collision)      // Wykonuje się, jak coś wychodzi z pola "trigger" pod kwadratem
    {
        if (collision.tag == "wall")    //czy to "coś" oznaczyłem jako ściana?
            canJump = false;            // jak tak, to nie dotyka już podłogi i nie może skakać
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;      // zaatakowało, więcej nie
        weapone.GetComponent<SpriteRenderer>().color = Color.red;      //szukamy części graficznej broni i zmieniamy jej kolor na czerwony
        weapone.GetComponent<WeaponeScript>().Damage(dmg);      //szukamy skryptu broni i aktywujemy funkcję zadającą obrażenia
        yield return new WaitForSeconds(attackDelay);       //czekamy czas "attackDelay" w sekundach, przerwa między atakami
        weapone.GetComponent<SpriteRenderer>().color = Color.white;     //szukamy części graficznej broni i zmieniamy jej kolor na biały
        canAttack = true;   //spowrotem można atakować
    }
}
