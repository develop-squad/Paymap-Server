using System;

namespace PAYMAP_BACKEND.Utils
{
    public static class DateTimeFormatter
    {
        public static string FormatDateTime(DateTime dt)
        {
            char[] chars = new char[21];
            Write2Chars(chars, 0, dt.Year % 1000);
            chars[2] = '.';
            Write2Chars(chars, 3, dt.Month);
            chars[5] = '.';
            Write2Chars(chars, 6, dt.Day);
            chars[8] = ' ';
            Write2Chars(chars, 9, dt.Hour);
            chars[11] = ':';
            Write2Chars(chars, 12, dt.Minute);
            chars[14] = ':';
            Write2Chars(chars, 15, dt.Second);
            chars[17] = ':';
            Write2Chars(chars, 18, dt.Millisecond / 10);
            chars[20] = Digit(dt.Millisecond % 10);

            return new string(chars);
        }

        private static void Write2Chars(char[] chars, int offset, int value)
        {
            chars[offset] = Digit(value / 10);
            chars[offset+1] = Digit(value % 10);
        }

        private static char Digit(int value)
        {
            return (char) (value + '0');
        }
    }
}