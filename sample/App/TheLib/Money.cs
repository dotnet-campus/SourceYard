using System;
using System.Collections.Generic;
using System.Text;

namespace TheLib
{
    /// <summary>
    /// 人民币转大写
    /// </summary>
    public class Money
    {
        public Money(double money)
        {
            _money = money;
        }

        public string ToCapital()
        {
            if (Math.Abs(_money) < 0.0001)
            {
                return "零元";
            }

            var str = GetIntPart();
            GetDecimalPart(str);
            return str.ToString();
        }

        private static readonly List<char> Uppers = new List<char>()
        {
            '零',
            '壹',
            '贰',
            '叁',
            '肆',
            '伍',
            '陆',
            '柒',
            '捌',
            '玖'
        };

        private static readonly List<char> Units = new List<char>()
        {
            '分',
            '角'
        };

        private static readonly List<char> Grees = new List<char>()
        {
            '元',
            '拾',
            '佰',
            '仟',
            '万',
            '拾',
            '佰',
            '仟',
            '亿',
            '拾',
            '佰',
            '仟',
            '万',
            '拾',
            '佰'
        };

        private readonly double _money;

        private StringBuilder GetIntPart()
        {
            StringBuilder str = new StringBuilder();
            var money = _money;

            for (int i = 0; money > 0.99999; i++)
            {
                var n = (int)(money % 10);
                str.Insert(0, Grees[i]);
                str.Insert(0, Uppers[n]);
                money = money / 10;
                money = money - (n / 10.0);
            }

            str = str.Replace("零亿", "亿零");
            str = str.Replace("零万", "万零");

            str = str.Replace("零拾", "零");
            str = str.Replace("零佰", "零");
            str = str.Replace("零仟", "零");

            str = str.Replace("零零", "零");
            str = str.Replace("零零", "零");

            str = str.Replace("零亿", "亿");
            str = str.Replace("零万", "万");
            str = str.Replace("零元", "元");

            return str;
        }

        private void GetDecimalPart(StringBuilder str)
        {
            var money = _money * 100;
            for (int i = 0; i < 2; i++)
            {
                var n = (int)(money % 10);
                if (n != 0)
                {
                    str.Insert(0, Units[i]);
                    str.Insert(0, Uppers[n]);
                }

                money = money / 10;
            }
        }
    }

}
