using LearnQuickTyping.Core.Interfaces.Repositories;

namespace LearnQuickTyping.Core.Data.Repositories;

public class TextRepository : ITextRepository
{
    private readonly string[] _practiceTexts = new string[]
    {
        """
        The Clockmaker’s Promise. In the quiet town of Bellmere, there lived an old clockmaker named Corin.His shop was small, tucked between a bakery and a bookshop, but inside it glowed with the warm ticking of a hundred clocks. One morning, a little girl named Elara walked in carrying a broken pocket watch. “It was my brother’s,” she said. “He left for the sky-ships last winter. It stopped the day he left.” Corin held the watch to his ear. No tick, not even a whisper.“Some clocks,” he said gently, “need a reason to start again.” He worked through the afternoon, polishing gears, replacing springs, humming old sea-songs as he labored.When he finally wound the watch, it trembled, hesitated, then ticked softly at first, then strong and steady.Elara’s eyes shone. “Will it keep time now?” Corin smiled. “Better than time, child. It will keep hope. ”She left the shop clutching the watch, and the ticking echoed after her like a promise.
        """,

        """
        Every autumn, a soft blue glow drifted over the Windmere Marsh. People claimed it was swamp gas or tricks of moonlight, but Mara knew better. She had seen the lantern herself years ago, bobbing just ahead of her when she was lost in the fog. It had guided her home. Tonight, she returned with a promise to keep. She carried her own lantern, warm and golden, and walked carefully into the mist. When the blue light appeared, she whispered, “Thank you.” The glowing lantern hovered, pulsing softly like a heart. Mara lifted her own lantern high. For a moment, the two lights shone together, blue and gold, and the marsh seemed to breathe. Then the blue lantern drifted away, not guiding her this time, but watching her go. Mara left the marsh feeling lighter, knowing some kindnesses echo long after they’re given.
        """,

        """
        On a quiet rooftop garden in a crowded city, Niko tended the last seedling of a nearly forgotten tree. The seed had been found in an old museum drawer, labeled only “Specimen 47.” Most people doubted it would sprout at all. But Niko watered it every dawn, shielded it from storms, and whispered stories of forests it had never seen. Weeks passed. Then, one morning, a tiny green shoot curled upward toward the sun. News spread quickly. Neighbors brought soil, pots, and even songs. The rooftop became a gathering place filled with laughter and hope. As the seedling grew taller, Niko realized the miracle wasn’t just the tree returning—it was the city remembering how to care. One day, when the tree finally unfurled its first broad leaf, the whole rooftop cheered. In a place built of steel and concrete, something ancient and gentle had taken root, reminding everyone that life still had room to grow.
        """
    };

    private readonly Dictionary<string, string[]> _difficultyTexts = new()
    {
        {
            "beginner", new[]
            {
                "the green duck met ben 3 times, kicked him 4 times | jim bit the thick red fruit 5 times, then rested 6 minutes | i give her 7 gems, 8 rings, and 4 bucks | eric cut the vine, kicked the bin, and then danced | the kid bet 5 bucks, but tim bet 6 bucks, i bet 7 bucks | he tried the trick 8 times, failed 3 times, succeeded 4 times | ben hit the drum, eric hummed the tune, and jim danced | the hen bit the nut, the duck bit the fig | i think the byte, the bit, and the juice fumed him | tim kicked the red bucket, then he rested in the hut",
                "eric met the thick duck 3 times, fed it 4 berries | the kid kicked 5 times, jumped 6 times, rested 7 minutes | i give ben 8 green gems, 4 red rings, and 5 bucks | jim tried the trick, bit the fruit, and then danced | the hen bet 6 bucks, the duck bet 7 bucks, i bet 8 bucks | he cut the vine 3 times, kicked it 4 times, buried it | tim hit the drum 5 times, eric hummed 6 tunes | ben drank the juice, bit the nut, and then rested | the red duck bit him 7 times, the hen bit him 8 times | i think the bit, the byte, and the trick funnied him",
                "ben kicked the bin 3 times, eric kicked it 4 times | the thick red hen met jim 5 times, bit him 6 times | i give the kid 7 gems, 8 bucks, and 4 green rings | tim tried the byte, cut the bit, and then rested | he bet 5 bucks, eric bet 6 bucks, ben bet 7 bucks | the duck kicked 8 times, jumped 3 times, danced 4 minutes | jim hit the drum, ben hummed the tune, and i danced | eric bit the nut, drank the juice, and then rested | the kid cut the vine 5 times, the duck bit it 6 times | i think the trick, the gem, and the ring fumed her",
                "tim met the green duck 3 times, fed it 4 berries | the kid bet 5 bucks, jim bet 6 bucks, eric bet 7 bucks | i give ben 8 red gems, 4 green rings, and 5 thick nuts | he kicked the bin, hit the drum, and then rested | the hen tried the trick 6 times, succeeded 7 times, failed 8 times | eric cut the fruit, bit the fig, and drank the juice | ben hummed 3 tunes, tim danced 4 minutes, and i rested | the thick duck kicked him 5 times, the red hen bit him 6 times | jim buried the vine, dug the dirt, and then rested | i think the byte, the bit, and the bucket fumed him"
            }
    },
        {
            "intermediate", new[]
            {
                "the green duck met ben 3 times, kicked him 4 times. jim bit the thick red fox 5 times, then rested 6 minutes. i give her 7 gems, 8 rings, and 4 bucks. eric cut the vine, kicked the bin, and then danced well. the kid bet 5 bucks, but tim bet 6 bucks, i bet 7 bucks. he tried the trick 8 times, failed 3 times, succeeded 4 times. ben hit the drum 9 times, eric hummed the tune, and jim danced. the hen bit the nut 2 times, the duck bit the fox. i think the slow wolf, the bit, and the juice fumed him. tim kicked the red bucket, then he rested in the low hut.",
                "eric met the thick wolf 3 times, fed it 4 berries now. the kid kicked 5 times, jumped 6 times, rested 7 minutes low. i give ben 8 red gems, 4 green rings, and 5 bucks. jim tried the slow trick, bit the fox, and then danced. the hen bet 6 bucks, the wolf bet 7 bucks, i bet 8 bucks. he cut the vine 3 times, kicked it 4 times, buried it well. tim hit the drum 5 times, eric hummed 6 slow tunes. ben drank the wine, bit the nut 2 times, and then rested. the red duck bit him 7 times, the hen bit him 9 times. i think the bit, theox, and the slow trick funnied him.",
                "ben kicked the bin 3 times, eric kicked it 4 times well. the thick red hen met jim 5 times, bit him 6 times now. i give the kid 7 gems, 8 bucks, and 4 green slow rings. tim tried the old byte, cut the bit, and then rested low. he bet 5 bucks, eric bet 6 bucks, ben bet 7 bucks now. the wolf kicked 8 times, jumped 3 times, danced 4 minutes. jim hit the drum 9 times, ben hummed the slow tune, and i danced. eric bit the nut 2 times, drank the wine, and then rested. the kid cut the vine 5 times, the fox bit it 6 times. i think the trick, the ox, and the old ring fumed her well.",
                "tim met the green fox 3 times, fed it 4 berries slow. the kid bet 5 bucks, jim bet 6 bucks, eric bet 7 bucks well. i give ben 8 red gems, 4 old rings, and 5 thick nuts now. he kicked the bin 9 times, hit the drum, and then rested low. the hen tried the trick 6 times, succeeded 7 times, failed 2 times. eric cut the wolf, bit the fox, and drank the wine now. ben hummed 3 slow tunes, tim danced 4 minutes, and i rested. the thick ox kicked him 5 times, the red wolf bit him 6 times. jim buried the vine 8 times, dug the old dirt, and then rested. i think the slow byte, the bit, and the bucket fumed him well."
            }

        },
        {
            "advanced", new[]
            {
                "The green duck met Ben 3 times, kicked him 4 times! Jim bit the thick red fox 5 times, then rested 6 minutes. I give her 7 gems, 8 rings, and 4 bucks (wow). Eric cut the vine, kicked the bin, and then danced well. The kid bet 5 bucks, but Tim bet 6 bucks, I bet 7 bucks! He tried the trick 8 times, failed 3 times, succeeded 4 times. Ben hit the drum 9 times <loud>, Eric hummed the tune, and Jim danced. The hen bit the nut 2 times, the duck bit the fox @noon. I think the slow wolf, the bit, and the juice fumed him (bad). Tim kicked the red bucket 10 times, then he rested in the low hut.",
                "Eric met the thick wolf 3 times, fed it 4 berries now! The kid kicked 5 times, jumped 6 times, rested 7 minutes low. I give Ben 8 red gems <shiny>, 4 green rings, and 5 bucks ($$$). Jim tried the slow trick, bit the fox, and then danced wild! The hen bet 6 bucks, the wolf bet 7 bucks, I bet 8 bucks (crazy). He cut the vine 3 times, kicked it 4 times, buried it well. Tim hit the drum 5 times, Eric hummed 6 slow tunes <nice>. Ben drank the wine 2 times, bit the nut 9 times, and then rested. The red duck bit him 7 times @midnight, the hen bit him 10 times! I think the bit, the ox, and the slow trick funnied him.",
                "Ben kicked the bin 3 times, Eric kicked it 4 times well! The thick red hen met Jim 5 times, bit him 6 times now. I give the kid 7 gems, 8 bucks, and 4 green slow rings <pretty>. Tim tried the old byte, cut the bit, and then rested low (tired). He bet 5 bucks, Eric bet 6 bucks, Ben bet 7 bucks now! The wolf kicked 8 times, jumped 3 times, danced 4 minutes wild. Jim hit the drum 9 times, Ben hummed the slow tune, and I danced. Eric bit the nut 2 times @sunset, drank the wine 10 times, and then rested. The kid cut the vine 5 times, the fox bit it 6 times (ouch)! I think the trick, the ox, and the old ring fumed her well.",
                "Tim met the green fox 3 times, fed it 4 berries slow! The kid bet 5 bucks, Jim bet 6 bucks, Eric bet 7 bucks well. I give Ben 8 red gems <rare>, 4 old rings, and 5 thick nuts now! He kicked the bin 9 times, hit the drum 10 times, and then rested low (phew). The hen tried the trick 6 times, succeeded 7 times, failed 2 times @dawn. Eric cut the wolf, bit the fox, and drank the wine now yum. Ben hummed 3 slow tunes, Tim danced 4 minutes, and I rested (nice). The thick ox kicked him 5 times, the red wolf bit him 6 times! Jim buried the vine 8 times, dug the old dirt, and then rested. I think the slow byte, the bit, and the bucket fumed him well."
            }
        },
        {
            "expert", new[]
            {
                "The quick brown fox jumps over the lazy dog near the bank of the river while the sun sets in the west casting long shadows over the hills and valleys beyond."
                "Eric met the thick wolf 3 times, fed it 4 berries now! The kid kicked 5 times, jumped 6 times—rested 7 minutes low. I give Ben 8 red gems <shiny>, 4 green rings, and 5 bucks ($$$). Jim tried the slow trick, bit the fox, and then danced wild! The hen bet 6 bucks; the wolf bet 7 bucks; I bet 8 bucks (crazy). He cut the vine 3 times, kicked it 4 times, buried it well. Tim hit the drum 5 times—Eric hummed 6 slow tunes <nice>. Ben drank the wine 2 times, bit the nut 9 times, and then rested? The red duck bit him 7 times @midnight; the hen bit him 10 times! I think the bit, the ox, and the slow trick funnied him.",
                "Ben kicked the bin 3 times—Eric kicked it 4 times well! The thick red hen met Jim 5 times; bit him 6 times now. I give the kid 7 gems, 8 bucks, and 4 green slow rings <pretty>. Tim tried the old byte, cut the bit, and then rested low (tired). He bet 5 bucks; Eric bet 6 bucks; Ben bet 7 bucks now! The wolf kicked 8 times, jumped 3 times, danced 4 minutes wild. Jim hit the drum 9 times—Ben hummed the slow tune, and I danced? Eric bit the nut 2 times @sunset, drank the wine 10 times, and then rested. The kid cut the vine 5 times; the fox bit it 6 times (ouch)! I think the trick, the ox, and the old ring fumed her well.",
                "Tim met the green fox 3 times—fed it 4 berries slow! The kid bet 5 bucks; Jim bet 6 bucks; Eric bet 7 bucks well. I give Ben 8 red gems <rare>, 4 old rings, and 5 thick nuts now! He kicked the bin 9 times, hit the drum 10 times, and then rested low (phew). The hen tried the trick 6 times, succeeded 7 times—failed 2 times @dawn? Eric cut the wolf, bit the fox, and drank the wine now yum. Ben hummed 3 slow tunes; Tim danced 4 minutes, and I rested (nice). The thick ox kicked him 5 times—the red wolf bit him 6 times! Jim buried the vine 8 times, dug the old dirt, and then rested. I think the slow byte, the bit, and the bucket fumed him well."
            }
        }
    };

    public string GetRandomText()
    {
        var random = new Random();
        int index = random.Next(_practiceTexts.Length);
        return _practiceTexts[index]
            .Replace("“", "\"") // Opening double quote
            .Replace("”", "\"") // Closing double quote
            .Replace("’", "'")  // Right single quote
            .Replace("‘", "'"); // Left single quote
    }
    public int GetWordCount()
    {
        return _practiceTexts.Sum(text => CountWords(text));

    }

    public int CountWords(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        int count = 0;
        bool inWord = false;
        for (int i = 0; i < value.Length; i++)
        {
            bool isWhiteSpace = Char.IsWhiteSpace(value[i]);
            if (!inWord && !isWhiteSpace)
                count++;
            inWord = !isWhiteSpace;
        }
        return count;
    }
}