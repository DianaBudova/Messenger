﻿using System.Text;

namespace Messenger.Common;

public static class CharSetForAudioMessage
{
    public static readonly char[] Characters = {
        '⠀', '⠁', '⠂', '⠃', '⠄', '⠅', '⠆', '⠇', 
        '⡀', '⡁', '⡂', '⡃', '⡄', '⡅', '⡆', '⡇',
        '⠈', '⠉', '⠊', '⠋', '⠌', '⠍', '⠎', '⠏',
        '⡈', '⡉', '⡊', '⡋', '⡌', '⡍', '⡎', '⡏',
        '⠐', '⠑', '⠒', '⠓', '⠔', '⠕', '⠖', '⠗',
        '⡐', '⡑', '⡒', '⡓', '⡔', '⡕', '⡖', '⡗',
        '⠘', '⠙', '⠚', '⠛', '⠜', '⠝', '⠞', '⠟',
        '⡘', '⡙', '⡚', '⡛', '⡜', '⡝', '⡞', '⡟',
        '⠠', '⠡', '⠢', '⠣', '⠤', '⠥', '⠦', '⠧',
        '⡠', '⡡', '⡢', '⡣', '⡤', '⡥', '⡦', '⡧',
        '⠰', '⠱', '⠲', '⠳', '⠴', '⠵', '⠶', '⠷',
        '⡴', '⡵', '⡶', '⡷', '⡸', '⡹', '⡺', '⡻',
        '⠸', '⠹', '⠺', '⠻', '⠼', '⠽', '⠾', '⠿',
        '⡼', '⡽', '⡾', '⡿', '⢀', '⢁', '⢂', '⢃',
        '⢄', '⢅', '⢆', '⢇', '⣀', '⣁', '⣂', '⣃',
        '⣄', '⣅', '⣆', '⣇', '⢈', '⢉', '⢊', '⢋',
        '⢌', '⢍', '⢎', '⢏', '⣈', '⣉', '⣊', '⣋',
        '⣌', '⣍', '⣎', '⣏', '⢐', '⢑', '⢒', '⢓',
        '⢔', '⢕', '⢖', '⢗', '⣐', '⣑', '⣒', '⣓',
        '⣔', '⣕', '⣖', '⣗', '⣘', '⣙', '⣚', '⣛',
        '⣜', '⣝', '⣞', '⣟', '⢠', '⢡', '⢢', '⢣',
        '⢤', '⢥', '⢦', '⢧', '⣠', '⣡', '⣢', '⣣',
        '⣤', '⣥', '⣦', '⣧', '⣨', '⣩', '⣪', '⣫',
        '⢨', '⢩', '⢪', '⢫', '⢬', '⢭', '⢮', '⢯',
        '⣰', '⣱', '⣲', '⣳', '⣴', '⣵', '⣶', '⣷',
        '⢰', '⢱', '⢲', '⢳', '⢴', '⢵', '⢶', '⢷',
        '⣸', '⣹', '⣺', '⣻', '⣼', '⣽', '⣾', '⣿'
    };

    public static string BuildString(uint minChars, uint maxChars)
    {
        if (minChars >= maxChars)
            return string.Empty;
        StringBuilder resultString = new();
        for (uint i = minChars; i < maxChars; i++)
            resultString.Append(CharSetForAudioMessage.Characters[Random.Shared.Next(0, CharSetForAudioMessage.Characters.Count() - 1)]);
        return resultString.ToString();
    }
}