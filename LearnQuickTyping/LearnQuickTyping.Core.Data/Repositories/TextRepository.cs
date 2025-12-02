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

    public string GetRandomText()
    {
        var random = new Random();
        int index = random.Next(_practiceTexts.Length);
        return _practiceTexts[index];
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