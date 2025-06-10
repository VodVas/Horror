using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public static class EnglishPhonemeParser
{
    private struct PhonemeRule
    {
        public string Pattern;
        public VisemeType Viseme;
        public float Duration;

        public PhonemeRule(string pattern, VisemeType viseme, float duration)
        {
            Pattern = pattern;
            Viseme = viseme;
            Duration = duration;
        }
    }

    // Правила для английских фонем с учетом контекста
    private static readonly PhonemeRule[] PhonemeRules = new[]
    {
        // Диграфы и специальные сочетания (проверяются первыми)
        new PhonemeRule(@"\bth", VisemeType.TH, 0.08f),
        new PhonemeRule(@"ch|tch", VisemeType.CH, 0.10f),
        new PhonemeRule(@"sh|sch", VisemeType.CH, 0.10f),
        new PhonemeRule(@"ng\b", VisemeType.KK, 0.08f),
        new PhonemeRule(@"ph", VisemeType.FF, 0.08f),
        new PhonemeRule(@"wh", VisemeType.UU, 0.10f),
        new PhonemeRule(@"qu", VisemeType.KK, 0.10f),
        
        // Гласные с контекстом
        new PhonemeRule(@"a[iy]|ei|ey", VisemeType.EE, 0.12f),
        new PhonemeRule(@"ea|ee|ie|e\b", VisemeType.EE, 0.12f),
        new PhonemeRule(@"oa|ow|ou", VisemeType.OO, 0.14f),
        new PhonemeRule(@"oo|ew|ue", VisemeType.UU, 0.14f),
        new PhonemeRule(@"oi|oy", VisemeType.OO, 0.12f),
        new PhonemeRule(@"au|aw", VisemeType.AA, 0.14f),
        
        // Одиночные согласные
        new PhonemeRule(@"[pbm]", VisemeType.PP, 0.08f),
        new PhonemeRule(@"[fv]", VisemeType.FF, 0.08f),
        new PhonemeRule(@"[td]", VisemeType.DD, 0.06f),
        new PhonemeRule(@"[kg]", VisemeType.KK, 0.08f),
        new PhonemeRule(@"[sz]", VisemeType.SS, 0.08f),
        new PhonemeRule(@"[nl]", VisemeType.NN, 0.06f),
        new PhonemeRule(@"r", VisemeType.RR, 0.08f),
        new PhonemeRule(@"[jy]", VisemeType.II, 0.08f),
        new PhonemeRule(@"w", VisemeType.UU, 0.08f),
        new PhonemeRule(@"h", VisemeType.AA, 0.06f),
        
        // Одиночные гласные
        new PhonemeRule(@"a", VisemeType.AA, 0.12f),
        new PhonemeRule(@"e", VisemeType.EE, 0.12f),
        new PhonemeRule(@"i", VisemeType.II, 0.12f),
        new PhonemeRule(@"o", VisemeType.OO, 0.12f),
        new PhonemeRule(@"u", VisemeType.UU, 0.12f),
        
        // Пунктуация для пауз
        new PhonemeRule(@"[,;:]", VisemeType.Silent, 0.3f),
        new PhonemeRule(@"[.!?]", VisemeType.Silent, 0.5f),
        new PhonemeRule(@"\s+", VisemeType.Silent, 0.1f),
    };

    public static List<TimedViseme> ParseText(string text, float totalDuration, float wordsPerMinute)
    {
        if (string.IsNullOrWhiteSpace(text)) return new List<TimedViseme>();

        text = text.ToLower();
        var phonemes = new List<(VisemeType viseme, float duration)>();

        int position = 0;
        while (position < text.Length)
        {
            bool matched = false;

            // Пробуем применить правила в порядке приоритета
            foreach (var rule in PhonemeRules)
            {
                var regex = new Regex(rule.Pattern);
                var match = regex.Match(text, position);

                if (match.Success && match.Index == position)
                {
                    phonemes.Add((rule.Viseme, rule.Duration));
                    position += match.Length;
                    matched = true;
                    break;
                }
            }

            // Если не нашли правило, пропускаем символ
            if (!matched)
            {
                position++;
            }
        }

        return ConvertToTimedVisemes(phonemes, totalDuration, wordsPerMinute);
    }

    private static List<TimedViseme> ConvertToTimedVisemes(
        List<(VisemeType viseme, float duration)> phonemes,
        float totalDuration,
        float wordsPerMinute)
    {
        if (phonemes.Count == 0) return new List<TimedViseme>();

        var result = new List<TimedViseme>();
        float totalPhonemeWeight = phonemes.Sum(p => p.duration);
        float timeScale = totalDuration / totalPhonemeWeight;

        // Корректируем временную шкалу с учетом скорости речи
        float speedFactor = 150f / wordsPerMinute; // 150 WPM как базовая скорость
        timeScale *= speedFactor;

        float currentTime = 0f;
        VisemeType lastViseme = VisemeType.Silent;

        foreach (var (viseme, duration) in phonemes)
        {
            // Избегаем дублирования одинаковых визем подряд
            if (viseme != lastViseme)
            {
                result.Add(new TimedViseme
                {
                    Time = currentTime,
                    Viseme = viseme,
                    Weight = GetVisemeWeight(viseme)
                });
                lastViseme = viseme;
            }

            currentTime += duration * timeScale;
            currentTime = Mathf.Min(currentTime, totalDuration);
        }

        // Добавляем финальную Silent визему
        if (result.Count > 0 && result[result.Count - 1].Viseme != VisemeType.Silent)
        {
            result.Add(new TimedViseme
            {
                Time = Mathf.Min(currentTime, totalDuration),
                Viseme = VisemeType.Silent,
                Weight = 1f
            });
        }

        return result;
    }

    public static float GetVisemeWeight(VisemeType viseme)
    {
        // Веса для разных типов визем
        return viseme switch
        {
            VisemeType.Silent => 0f,
            VisemeType.PP => 0.9f,
            VisemeType.FF => 0.8f,
            VisemeType.TH => 0.7f,
            VisemeType.DD => 0.8f,
            VisemeType.KK => 0.85f,
            VisemeType.CH => 0.8f,
            VisemeType.SS => 0.7f,
            VisemeType.NN => 0.6f,
            VisemeType.RR => 0.7f,
            VisemeType.AA => 1f,
            VisemeType.EE => 0.9f,
            VisemeType.II => 0.8f,
            VisemeType.OO => 0.95f,
            VisemeType.UU => 0.9f,
            _ => 0.8f
        };
    }
}