using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TalkData 
{
    static List<int> accessLevel = new List<int>();
    public static List<int> AccessLevel { get { return accessLevel; } }


    static List<string> titles = new List<string>();
    public static List<string> Titles { get { return titles; } }


    static List<string> texts = new List<string>();
    public static List<string> Texts { get { return texts; } }

    public static int MaxTalks { get { return texts.Count; } }

    static bool initialized = false;

    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        // 0

        accessLevel.Add(1);
        titles.Add("Arrival");
        texts.Add("Welcome to Ghelinnal City - or what's left of it. You have our thanks for defeating" +
            " those goblins on the way in. We probably could've handled them ourselves, but, well, a" +
            " siege is a siege. And we did lose a few."
            + "\n\n" +
            "We don't have that many more to lose, either. Altogether, only fifteen of us remain. We'd" +
            " leave but this is the land given to us by our goddess, Sehanine Moonbow, in an ancient" +
            " ritual generations ago."
            + "\n\n" +
            "Yes, that is an elven goddess.Are you surprised? We are all elves here, decendents of a" +
            " prosperous nation that perished in a calamity."
            + "\n\n" +
            "There's more to the tale, of course, but could you do us a favor? There are catacombs" +
            " below the city, and we believe them to be corrupted. Normally, the pointy sticks of the" +
            " goblins are turned by our magical hides, but these pierce and kill. Something must be" +
            " interfering with the pact of our ancestors, and it is robbing us of a portion of our" +
            " blessing."
            + "\n\n" +
            "It's taboo for us to go ourselves, and in our weakened state, we're even more vulnerable." +
            " You seem capable, though. Could you take a look around?");

        // 1

        accessLevel.Add(2);
        titles.Add("First Delve");
        texts.Add("Sorry, you caught me thinking about our friend here. These are serious wounds, and" +
            " my healing ability is wearing thin. I don't know what to do about this black poison. "
            + "\n\n" +
            "Do you know who that robed figure was with the goblins ? She wasn't goblinoid, not even" +
            " a hobgoblin. Well, let me know if you see another green robe like hers."
            + "\n\n" +
            "What did you find in the catacombs ? Ah, I see, so you've not made it very far yet. Keep" +
            " on plugging further in, and if you need to rest, just come topside and we'll take care" +
            " of you. We even have a few supplies left. You're welcome to them, but we do have to " +
            "charge a bit. Traders do make it through here, and they bring some of the oddest wares."
            + "\n\n" +
            "Hey now, put that away! Don't you know that silver is forbidden in this city? If the " +
            "chieftain saw you with that star he'd toss you out in a moment."
            + "\n\n" +
            "A key ? Oh, of course! No wonder we couldn't enter the catacombs. "
            + "\n\n" +
            "The statues you saw inside must be of warriors long forgotten. Knights of the city, who" +
            " bound their souls to be our protectors. They called themselves Baelnorn. We've only ever" +
            " met one. You should talk to Arthon - he's the current caretaker of our local relic.");

        // 2
        accessLevel.Add(3);
        titles.Add("Arthon");
        texts.Add("Hullo! Sir Reghinael, it looks like we will have visitors after all! I must thank" +
            " Ridara later for sending company. Come in, come in! Let me give you a hand with that" +
            " rope ladder. So exciting! I must put some tea on. Welcome to the Seventh Star."
            + "\n\n" +
            "Cozy isn't it? Well, what do expect? This was just one small bed chamber from the Temple" +
            " ruins below. No, I don't think it will fall out of the sky. This rock has been floating" +
            " overhead for as long as I've been alive."
            + "\n\n" +
            "Oh, not much longer than any of you, I'd imagine. When we accepted the blessing of " +
            "Sehanine, we lost the long life of our wood elf kin. We're much more mortal - well," +
            " except Sir Reghinael here."
            + "\n\n" +
            "He's our relic, and I'm his caretaker.I know he looks like a corpse. Well, he is. A " +
            "corpse, I mean, but he's a really good one! He's a Baelnorn, a sort of warrior - lich" +
            " that devoted himself to the protection of the City. He's immortal now, but he was " +
            "consecrated a long time ago, and the world has changed a lot since then."
            + "\n\n" +
            "Kind of. He's a bit off in the head. He sleeps there most of the time. Every few years, " +
            "though, he'll perk up with some bit of prophesy. Sometimes he's even lucid enough to choose" +
            " a priest to act as his next caretaker."
            + "\n\n" +
            "He picked me the last time. Yup, very lucky."
            + "\n\n" +
            "Well, I suppose we can try. Hey, Sir Rheghinael, we have visitors! C'mon, wake up! "
            + "\n\n" +
            "… Oh.");
        
        // 3
        accessLevel.Add(3);
        titles.Add("Prophesy");
        texts.Add("The Prophesy of Reghinael, Baelnorn of the Galathi"
            + "\n\n" +

            "Dragon pact. Dragon pact. Dragon pact."
            + "\n\n" +
            " Dragon pact: Little elf. Share the debt. Nations doomed. Dragon pact."
            + "\n" +
            " Dragon pact: Bound to throne. Fraternal rule. Mark of death. Dragon pact."
            + "\n" +
            " Dragon pact: Maiden hero. Sealed diseased. Missing suffering. Dragon pact."
            + "\n\n" +

            "Dragon pact. Dragon pact. Dragon pact."
            + "\n\n" +
            " Three dragons. Three pacts."
            + "\n" +
            " Three nations. Four talons. Four seals."
            + "\n" +
            " Break the seal. Open door. Beginning ending."
            + "\n\n" +
            "Dragon pact. Dragon pact. Dragon pact.");

        //4
        accessLevel.Add(4);
        titles.Add("Jirindace'el");
        texts.Add("Thank you for releasing me from that curse. Long have I watched and protected these" +
            " halls. None have passed without my permitting it, until recently."
            + "\n\n" +
            "Hidden from the dead, somehow. Like me, touched by death, but I could neither hear nor see" +
            " them. Just a presence that passed, a powerful one. More came afterward, hard to say how" +
            " many. They have done something below. It has twisted my pact."
            + "\n\n" +
            "To defend these catacombs. As I did when we fought."
            + "\n\n" +
            "I am Jirindace'el, the Warden. By your looks, I am not the first Baelnorn you" +
            " have met, though."
            + "\n\n" +
            "Sir Reghinael is still alive ? …You know what I mean. He was one of our best, one of " +
            " the original four. He has been away from these tunnels since the cataclysm. I had thought" +
            " he was lost. The cataclysm buried many of us. A few survivors became twisted in the" +
            " maelstrom of magic. A fragment must still remain in him though. He is strong."
            + "\n\n" +
            "Yes, I do think there is something wrong below. I have felt it, gnawing and grasping at " +
            "the bounds that anchor my soul to this place. Sir Reghinael must be feeling the same " +
            "torture. If you can stop whatever force is below, you may restore his sanity."
            + "\n\n" +
            "Below this room? A chamber for the royal dead. Be careful, though. I know not what " +
            "destruction the cataclysm has wrought.");


        // 5
        accessLevel.Add(4);
        titles.Add("Laboratory");
        texts.Add("You find a book in the library entitled Those Who Watch."
            + "\n\n" +
            "Deep in the earth, the elves of Ghelinnal City practiced their art, blending magic" +
            " with knowledge. With the assistance of the patinnil Golgaq, they became masters of " +
            "the binding of souls, and from this practice were created the Baelnorn. Requiring a" +
            " strong and willing soul, the Baelnorn were bound to eternal service on behalf of" +
            " the people of Ghelinnal City. They were bound protectors, and, being immortal, served" +
            " as keepers of lore and knowledge."
            + "\n\n" +
            "Baelnorn were permitted by Sehanine Moonbow, who normally abhors the undead. It helped " +
            "that the Baelnorn were frequently employed as guardians of tombs, watching over the" +
            " dead as they slept, ensuring they stayed that way."
            + "\n\n" +
            "The process is irreversible, but through it, a Baelnorn is gifted immense" +
            " ability. No two Baelnorn are identical. Much of their formation depends on the character" +
            " of the soul being bound. Some become great warriors, others, powerful mages. Every " +
            "Baelnorn is tied to a phylactery, an artifact that is very difficult to destroy. So long" +
            " as the phylactery exists, the Baelnorn will persist."
            + "\n\n" +
            "It is theorized that it may be possible to create a Baelnorn without the requirement" +
            " of a phylactery. Thus far, no Baelnorn has survived the process."
            + "\n\n" +
            "An imitation of the Baelnorn process can be found among men, who, through evil " +
            "necromancy, rip souls from poor victims and bind them to machines of war. This is" +
            " called Magitek.");

        // 6
        accessLevel.Add(4);
        titles.Add("Patinnils");
        texts.Add("You find a book in the library entitled Ghelinnal City."
            + "\n\n" +
            "One of three Galathi nations, Ghelinnal City was originally founded by wood elves. It" +
            " was a thriving kingdom that flourished with trade from humans, goblins, and dwarves" +
            " alike. There was a dispute that arose, however, when its people began to rely on" +
            " Patinnil, massive gems of power, created with Pact Magic. A contingent of wood elves" +
            " departed the city, forming a naturalist community around the great tree Cefthonoth."
            + "\n\n" +
            "Four Patinnil are known to support the city:\n\n" +
            "Ipsennia - a blue crystal, housed at the Sarethi Tower, where most Patinnil research " +
            "is accomplished. Within this crystal was bound powerful elemental beings of the air, and" +
            " it provided protection to the city from scrying magic, severe weather, and airborn assault."
            + "\n\n" +
            "Golgaq - a dark gray crystal, buried in the catacombs. Powerful earth elementals were" +
            " bound to it. With its aid, the dead slept peacefully and the earth above grew" +
            " abundantly. It permitted the creation of Baelnorn."
            + "\n\n" +
            "Venlarren - a deep blue - green crystal. This was housed in Solonor's Watch, and had" +
            " powerful water elementals bound to it. Its power dug into the waterways, and provided" +
            " communication with the other Galathi nations. Scouts of this tower were said to have" +
            " piercingly good vision, and could divine the intent of those who would advance upon the city."
            + "\n\n" +
            "Irinnicar - a pale red crystal. Powerful fire elementals were bound to this Patinnil," +
            " housed in the temple to Sehanine Moonbow. It was capable of drawing fatigue, despair," +
            " and sorrow into itself.");

        // 7
        accessLevel.Add(5);
        titles.Add("Paurkin");
        texts.Add("A mountain underground? Well, that's a new one. Ever since the cataclysm shattered" +
            " Ghelinnal City's patinnil, magic's been crazy everywhere. It wouldn't surprise me if" +
            " one of the exploded underground and ripped open dimensions to completely different" +
            " worlds. Who knows where you'll go as you go further down?"
            + "\n\n" +
            "Reminds me of something, though. Borodin and I were hanging out with Olinniva, camping out" +
            " in the Briarwood. Yeah, don't tell my dad about that - he doesn't care for the 'tree elves'" +
            " much."
            + "\n\n" +
            "Anyway, he was telling me about this one elf from way back called Gilsilef. He was the" +
            " first elf to learn Pact Magic. It's said that he climbed a really tall mountain, called" +
            " Mount Orthax, and when he got to the summit, there was this dragon, but like, it was all" +
            " one big diamond. Inside and out, the whole thing, a diamond."
            + "\n\n" +
            "This diamond dragon spoke into Gilsilef's mind, telepathy or something like that. I guess" +
            " it was like the remains of a dragon, but that the soul still remained inside it."
            +"\n\n" +
            "It called itself Paurkin, the first and greatest of all gem dragons. And it taught" +
            " Gilsilef how to bind souls to things like gems."
            + "\n\n" +
            "When Gilsilef came down from the mountain, he returned Galagar.That's one of the three" +
            " old Galathi nations, south of the Briarwood. Ghelinnal was another one, back in the day." +
            " Well, all three got together in a thing called the Galathi Concord, and Gilsilef agreed" +
            " to teach all three what he knew."
            + "\n\n" +
            "Crazy, huh? Seriously, though, you can trust good ol' Finnel. Borodin does! Don'cha, you" +
            " big lump o' lard!");

        // 8 

        accessLevel.Add(6);
        titles.Add("The Cataclysm");
        texts.Add("How did you get up here? You're lucky I was flying by and saw you. "
            + "\n\n" +
            "Here, I brought these for you. They are fragments of Irinnicar, the great red patinnil that" +
           " once blessed the Temple of Sehanine Moonbow. Few fragments remain, so use them only when" +
            " most necessary."
            + "\n\n" +
            "Good view, isn't it? Didn't you say you came from Old Briar? That's it, just down there." +
            " Looks tiny, doesn't it ?"
            + "\n\n" +
            "See those mountains beyond ? Those are called the Dragon Spine Mountains. In the east" +
            " see those really tall peaks that stick out? That's where they say Cyrind Castle is, " +
            "and the beginning of the Dawn Age."
            + "\n\n" +
            "The stone giants that inhabited Cyrind Castle had an artifact called the Smoking Urn," +
            " said to contain the ashes of Chronepsis, the dragon guardian of Fate. It was stolen by" +
            " a cult, who unstoppered it to bring their god Tiamat to Valinda'ar."
            + "\n\n" +
            "Black smoke filled the land, not unlike what happened when you opened the catacombs, but " +
            "across the entire continent. A portal opened in the midst of Cyrind castle, and Tiamat" +
            " began to come through. The stone giants, however, threw great boulders at the portal and" +
            " smashed it. The portal collapsed, but it sent a shockwave throughout the land."
            + "\n\n" +
            "It opened the Black Rift in the far north. The ocean surged and swallowed land to the" +
            " east, creating the Bay of Sha. The Farengrul nation of men and their allies the Galagar" +
            " ceased to be."
            + "\n\n" +
            "Here in Ghelinnal City, the shockwave tore apart our patinnil, including Irinnicar," +
            " each exploding in destructive elemental magic. Why do you think the catacombs" +
            " are taboo? Golgaq is probably in several shards. There's no telling what odd portals" +
            " and horrors may have opened below.");

        // 9 

        accessLevel.Add(7);
        titles.Add("Calengund");
        texts.Add("The werebears have forgotten much of their own lore. Their memory is short, as " +
            "are their lives. But I still remember. The Calengund - or the Green Prince in your" +
            " tongue - I know of him."
            + "\n\n" +
            "He took one of my people as his bride, uniting the Thoneldim and the people of Ghelinnal" +
            " for a time. You know the Lay of the Calengund. It is a popular and familiar tale," +
            " but there is more."
            + "\n\n" +
            "The Calengund and his bride Delia Liadon were traveling the Feywild with their son at" +
            " the time of the Cataclysm. They became trapped there, unable to return to their" +
            " people. When they finally escaped, they found their beloved city laid to waste, and" +
            " its people scattered and near extinction. The magic they relied on now drove them" +
            " to empty, craven husks."
            + "\n\n" +
            "Beseeching Sehanine Moonbow, the Calengund and his bride made a pact with the goddess." +
            " The last great practitioner of Pact Magic among the Ghelinnal, he bound their souls" +
            " in the depths of the catacombs, and in exchange the goddess blessed her people with" +
            " invulnerable werebear forms."
            + "\n\n" +
            "Their son, however, was been stolen from them, just as they had set foot again in" +
            " Valinda'ar. None knew where he went. Delia's sister Ielennia spent half a lifetime" +
            " seeking him out."
            + "\n\n" +
            "But I have discerned it. By a twist of fate, he returned to us, wild and with no" +
            " memory, a curse of the Feywild. But he bore his father's sword. He and I trained " +
            "together, here at Morthondoth, for were both young and full of destiny. He might have " +
            "become Master Ordiner instead of me, but his heart yearned for a home he knew not."
            + "\n\n" +
            "Meanwhile, the Calengund's mind continues to drift between life and death, between the" +
            " material world and the realm of fey spirits. You have seen it too, have you not? Or" +
            " have you also forgotten?");

        // 10

        accessLevel.Add(8);
        titles.Add("Floating Continent");
        texts.Add("Do you seek what those before you sought? Do you crave power?"
            + "\n\n" +
            "I can give this to you. See, my power has torn the earth asunder. This continent is hurled" +
            " skyward, like none other since the avariel lifted their fortress from Mount Illuhaheth." +
            " Chaos demands this of me, and so I do it. I pluck the Great Black Root from the earth" +
            " and black death pours out."
            + "\n\n" +
            "Realms converge in this place. Wild realms like I knew once, and dark realms of shadow. I" +
            " bring them all into being. Can one place contain them all? Or will I tear apart" +
            " existence itself?"
            + "\n\n" +
            "See my bed and that of my lady. My beautiful Delia, tortured as I am by others' greed. Our" +
            " bed is shadow made poison. We have become monsters."
            + "\n\n" +
            "A Talon has clawed open the doorway. Have you followed? Inside the dragon awaits, but" +
            " lust has devoured him from within."
            + "\n\n" +
            "But the dragon has been defeated, and its Master can no longer reach Valinda'ar. I shall" +
            " shut the doorway. My people are healed, and now I can rest.");

        // 11

        accessLevel.Add(8);
        titles.Add("Rathazzan");
        texts.Add("How can this be? Dare you challenge The Emerald Claw? It will not perish so" +
            " easily. I have waited centuries for my moment. Immortality is within reach, you have" +
            " only delayed the inevitable."
            + "\n\n" +
            "I must say, I am disappointed in you, Iorvograx. A Talon you were, but you are nothing" +
            " more to me than a discarded scale. "
            + "\n\n" +
            "And you, you see before you the next Lord of Death. My sister is naught but dust, nor can" +
            " your paltry gods do anything to stop me. Go on, run back to your werebear friends with" +
            " this honor: Today, you have seen your nemesis.");



    }

}
