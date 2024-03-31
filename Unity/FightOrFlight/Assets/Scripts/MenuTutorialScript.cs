using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuTutorialScript : MonoBehaviour
{
    public Image image;
    public Text text;

    int currentGuideId = 1;

    // Start is called before the first frame update
    void Start()
    {
        switchGuide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextClick()
    {
        currentGuideId++;
        if (currentGuideId > 16)
        {
            currentGuideId = 1;
        }
        switchGuide();
    }

    public void PreviousClick()
    {
        currentGuideId--;
        if (currentGuideId < 1)
            currentGuideId = 16;
        switchGuide();
    }

    public void BackClick()
    {
        SceneManager.LoadScene("SceneMenu");
    }

    private void switchGuide()
    {
        switch(currentGuideId)
        {
            case 1:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Touch any free place of the left part of screen and move your finger to direction " +
                    "you want to move";
                break;
            case 2:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Red (central) button is for atack. The atack won\'t be made if your" +
                    " weapon is recharging now. For some types of weapon and monsters it\'s better to " +
                    "click as fast as it is possible. \n\n" +
                    "Green (under the atack button) is for using instrument (for himans)" +
                    " or special ability (for monsters). \n\n" +
                    "Blue is for interacting to objects (activate generator, escape using lift or " +
                    "take item near you). Disappears if there is no objects to intetact with.";
                break;
            case 3:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "If your health is down 0, your character dies. People can heal themselves " +
                    "using first aid kits (white). " +
                    "Black aid kit makes you faster, but damages you, didgeridoo is also dangerous for it\'s user" +
                    "\n\nMonsters heal during time";
                break;
            case 4:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "There are 2 teams \n\n\tHumans:\n" +
                    "Kill or escape every monster. If you can\'t find normal weapon, it\'s better to run. " +
                    "To win need to kill every monster or find generator and escape using any lift. " +
                    "\n\n\tMonsters:\n" +
                    "Kill every human to win. You are dangerous result of dark experiments... " +
                    "You have special abilities and high damage. But hunter can become victim if " +
                    "your opponent has good weapon and tactics. Defend generator, without of it lifts " +
                    "can\'t work and humans can\'t win in a quick way!";
                break;
            case 5:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Generator spawns in random room, this object is target position for every team. " +
                    "Finding geretator is key for win. " +
                    "\nUse blue button to activate and make lifts enabled. " +
                    "Also it makes this place not so dark as at the begining.";
                break;
            case 6:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Lifts are situated in every corner of this place, but using blue button " +
                    "gives no effect until generator is not activated. \nHumans need to escape so after " +
                    "generator activated you need to run to nearest lift. Monsters can\'t win if somebody escapes!\n\n" +
                    "Lifts become enabled only after generator activated and complex becomes not so dark.";
                break;
            case 7:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Toad is a huge frog-alike creature with medium damage. \n\n" +
                    "Using green button while walking makes a jump. Recharge is about 2 seconds. \n\n" +
                    "Weakness: chainsaw";
                break;
            case 8:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Rat is a huge... rat. Fast with big damage, but can be easily killed by human";
                break;
            case 9:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Slither can\'t be damaged by any weapon, may block some locations by it\'s own body. \n\n" +
                    "If you see it, just run, because it can make a trap for you using tactics\n\n" +
                    "Weakness: bombs, dynamite and didgeridoo";
                break;
            case 10:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Black goo is something really strange. \n" +
                    "Special ability: walking throgh solid objects (to switch modes use ability button). " +
                    "Damages the monster, so this ability can\'t be used for a long time. \n\n" +
                    "Weakness: women";
                break;
            case 11:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Weapon is very different, for some classes ammo is limited. \n\n" +
                    "Chainsaw, knife, pick are effective, but monsers can also hit you while that. " +
                    "Just spam the attack button to use this weapon in most effective way. \n\n" +
                    "Pistol, reagenst, sprayer and some other types are more safe\n\n" +
                    "Machine gun is a very effective thing, if you find it - take and shoot into nearest monster! \n\n" +
                    "P.S. Slither doesn\'t care about this page of tutorial";
                break;
            case 12:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Bomb can kill everyone in it\'s radius, use carefully. Is very useful against slither, because " +
                    "it is the only way to damage this monster.";
                break;
            case 13:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "First aid kit heals you, the black one (stimulator) damages, but makes faster\n\n" +
                    "Advice: miner needs stimulant more than any other human";
                break;
            case 14:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Don\'t forget about timer! If game is finished and nobody wins, the game will finish in a draw.";
                break;
            case 15:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Invisiblity hat. Effective if generator is already on, in darkness monsters feel your soul and" +
                    " can find you (but it takes more time and). It is very useful (sometimes), makes you speed lesser";
                break;
            case 16:
                image.sprite = Resources.Load<Sprite>("otval");
                text.text = "Didgeridoo damages your body, but makes monsters to suffer. For few secons they " +
                    "can\'t move normally, slither is the most weak against this. \n" +
                    "If toad jumps you can use it and the green monster flyes away. Have fun, but don\'t forget, " +
                    "don\'t use it too much.";
                break;
        }
    }
}
