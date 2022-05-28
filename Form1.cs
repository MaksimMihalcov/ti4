using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private class Buf
        { // Вспомогательный класс для хранения 3-х значений
            public long d;
            public long x;
            public long y;
            public Buf(long d, long x, long y)
            {
                this.d = d;
                this.x = x;
                this.y = y;
            }
        }

        private static Buf GetExtendGcd(long a, long b)
        {  // Расширенный алгоритм Евклида
            if (b == 0)
            {
                return new Buf(a, 1, 0);
            }
            else
            {
                Buf tmp = GetExtendGcd(b, a % b);
                long d = tmp.d;
                long y = tmp.x - tmp.y * (a / b);
                long x = tmp.y;
                return new Buf(d, x, y);
            }
        }

        private static long GetHash(string s, long n)
        {
            long h = 100;
            for (int i = 0; i < s.Length; i++)
            {
                h = (h + (int)s[i]) * (h + (int)s[i]) % n;
            }
            return h;
        }

        public static bool IsCoprime(long a, long b)
        {  // Являются ли числа взаимно простыми
            if (a == b)
            {
                return a == 1;
            }
            else
            {
                if (a > b)
                {
                    return IsCoprime(a - b, b);
                }
                else
                {
                    return IsCoprime(b - a, a);
                }
            }
        }

        private static bool IsPrime(long a)
        { // Является ли число простым
            for (long i = 2; i <= Math.Sqrt(a); i++)
            {
                if (a % i == 0)
                {
                    return false;
                }
            }
            return true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var r = (long)pval.Value * (long)qval.Value;
            if (radioButton1.Checked)
            {
                var f = ((long)pval.Value - 1) * ((long)qval.Value - 1);
                if (pval.Value > 1 && qval.Value > 1)
                {
                    if (IsPrime((long)pval.Value))
                    {
                        if (IsPrime((long)qval.Value))
                        {
                            if (((long)exp.Value > 1) && (long)exp.Value < f)
                            {
                                if (IsPrime((long)exp.Value) && IsCoprime((long)exp.Value, f))
                                {
                                    Buf temp = GetExtendGcd(f, (long)exp.Value);
                                    long d = temp.y;
                                    if (d < 0)
                                    {
                                        d += f;
                                    }
                                    MessageBox.Show($"Ko по алгоритму Евклида: e:{exp.Value} r:{r}");
                                    MessageBox.Show($"Kc по алгоритму Евклида: d:{d} r:{r}");
                                    long hash = GetHash(textBox1.Text, r);
                                    long s = Power(hash, d, r);
                                    MessageBox.Show($"Хеш: {hash};\nПодпись: {s}");
                                }
                                else
                                {
                                    MessageBox.Show("Экспонента не взаимно простая с ф.Эйлера!");
                                }
                            }
                            else
                                MessageBox.Show("Экспонента не вписывается в интервал!");
                        }
                        else
                            MessageBox.Show("Число q не простое!");  
                    }
                    else
                        MessageBox.Show("Число p не простое!");
                }
                else
                    MessageBox.Show("p и q должны быть больше 1!");
            }
            else
            {
                long newHash = GetHash(textBox1.Text, r);
                if (newHash == Power((long)ecpval.Value, (long)exp.Value, r))
                {
                    MessageBox.Show("Подпись подтверждена");
                }
                else
                {
                    MessageBox.Show("Подпись недействительна");
                }
                MessageBox.Show($"Хеш сообщения: {newHash}\n Хэш по подписи: {Power((long)ecpval.Value, (long)exp.Value, r)}");
            }
        }

        private static long Power(long x, long y, long N)
        {  //Алгоритм быстрого возведения в степень
            if (y == 0) return 1;
            long z = Power(x, y / 2, N);
            if (y % 2 == 0)
                return (z * z) % N;
            else
                return (x * z * z) % N;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ecpval.Enabled = false;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            //ecpval.Enabled = false;
            //pval.Enabled = true;
            //qval.Enabled = true;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            //ecpval.Enabled = true;
            //pval.Enabled = false;
            //qval.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(openFileDialog1.FileName);
            }
        }
    }
}