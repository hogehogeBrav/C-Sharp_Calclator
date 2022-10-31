using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calclator
{
    public partial class Calclator : Form
    {
        // 文字の上書きができるか(数字一桁の際)
        private bool isOverwrite = true;
        private bool inCalc = false;
        // イコール押下後かどうか
        private bool inEqual = false;
        // 計算結果保持
        float tmp = 0;

        public Calclator()
        {
            InitializeComponent();
        }

        // 数字入力ボタン 0
        private void ZeroClick(object sender, EventArgs e)
        {
            if ((isOverwrite || textarea.Text == "0") && !inEqual) textarea.Text = "0";
            else if(!inEqual) textarea.Text += "0";
        }
        // 数字入力ボタン 1
        private void OneClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 1);
        // 数字入力ボタン 2
        private void TwoClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 2);
        // 数字入力ボタン 3
        private void ThreeClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 3);
        // 数字入力ボタン 4
        private void FourClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 4);
        // 数字入力ボタン 5
        private void FiveClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 5);
        // 数字入力ボタン 6
        private void SixClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 6);
        // 数字入力ボタン 7
        private void SevenClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 7);
        // 数字入力ボタン 8
        private void EightClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 8);
        // 数字入力ボタン 9
        private void NineClick(object sender, EventArgs e) => NumButtonAction(isOverwrite, 9);

        // 数字ボタンアクション
        public void NumButtonAction(bool overwrite, int num)
        {
            // 計算中フラグオン
            inCalc = true;
            // イコール後以外の処理
            // 上書きフラグ（一桁未満）の際は数字上書き
            if (overwrite && !inEqual)
            {
                textarea.Text = num.ToString();
                this.isOverwrite = false;
            }
            // それ以外は数字を後ろに結合
            else if (!inEqual)
            {
                textarea.Text += num.ToString();
            }
        }

        // ドット
        private void DotClick(object sender, EventArgs e)
        {
            // ドットが存在しない時のみ挿入
            if (!textarea.Text.Contains(".") && !inEqual)
            {
                textarea.Text += ".";
                isOverwrite = false;
            }
        }

        //符号変更
        private void SignChangeClick(object sender, EventArgs e) => textarea.Text = SignChange(textarea.Text, inEqual);
        // 符号変更メソッド
        private string SignChange(string textarea, bool equal)
        {
            if (textarea.StartsWith("-") && !equal)
            {
                return textarea.Remove(0, 1); // 負の際、-削除
            }
            else if (textarea == "0" || textarea == "エラー" || equal)
            {
                return textarea; // 0、エラー表示の際とイコールの後は符号を変更しない
            }
            else
            {
                return "-" + textarea; // 正の際、-削除
            }
        }

        // 一文字削除
        private void DeleteClick(object sender, EventArgs e)
        {
            // 計算結果が0の際とイコールの後は数字削除しない
            if(textarea.Text != "0" && !inEqual)
            {
                // 入力数字が一桁の場合は0を挿入して上書きオン
                if (textarea.Text.Length == 1)
                {
                    textarea.Text = "0";
                    isOverwrite = true;
                }
                // 2桁以上は一文字削除
                else
                {
                    textarea.Text = textarea.Text.Substring(0, textarea.Text.Length - 1);
                }
            }
        }

        // 符号ボタン
        // 和
        private void PlusClick(object sender, EventArgs e) => SignButtonAction("+");
        // 差
        private void MinusClick(object sender, EventArgs e) => SignButtonAction("-");
        // 積
        private void MultipliedClick(object sender, EventArgs e) => SignButtonAction("×");
        // 商
        private void DividedClick(object sender, EventArgs e) => SignButtonAction("÷");
        // 符号ボタンアクションメソッド
        public void SignButtonAction(string sign_value)
        {
            if (textarea.Text == "0" || inEqual) inCalc = true;
            // エラー表示の際は計算しない
            if (textarea.Text == "エラー") inCalc = false;
            // 計算処置
            if (inCalc)
            {
                tmp = Calc(sign.Text, textarea.Text, tmp);
                EqualCheck(sign.Text);
                formula.Text += sign.Text + textarea.Text;
                textarea.Text = tmp.ToString();
                inCalc = false;
                inEqual = false;
                isOverwrite = true;
                sign.Text = sign_value; // 計算した符号を符号エリアに挿入
            }
            else
            {
                sign.Text = sign_value;
            }
        }
        public void EqualCheck(string value)
        {
            if (value == "=")
            {
                formula.Text = "";
                sign.Text = "";
            }
        }

        // オールクリア
        private void AllClearClick(object sender, EventArgs e) => AllClear();
        // クリアボタン
        private void ClearClick(object sender, EventArgs e)
        {
            // イコールの後のみオールクリア
            if(sign.Text == "=") AllClear();
            else
            {
                textarea.Text = "0";
                isOverwrite = true;
            }
        }
        // オールクリア関数
        public void AllClear()
        {
            textarea.Text = "0";
            tmp = 0;
            sign.Text = "";
            formula.Text = "";
            isOverwrite = true;
            inEqual = false;
        }

        // イコールボタン
        private void EqualClick(object sender, EventArgs e)
        {
            // 演算子で分岐
            tmp = Calc(sign.Text, textarea.Text, tmp);
            // 以前の演算子がイコール以外の際、式を結合する
            if (sign.Text != "=") formula.Text += sign.Text + textarea.Text;
            // ゼロ除算の際のエラー表示
            textarea.Text = DividedError(sign.Text, textarea.Text, tmp);
            inEqual = true;
            isOverwrite = true;
            sign.Text = "=";
        }

        // 除算エラー表示
        private string DividedError(string sign, string textarea, float tmp)
        {
            if (sign == "÷" && textarea == "0") return "エラー";
            else return tmp.ToString();
        }

        // 計算
        public float Calc(string sign, string textarea, float tmp)
        {
            switch (sign)
            {
                case "":
                    return float.Parse(textarea);
                case "+":
                    return tmp + float.Parse(textarea);
                case "-":
                    return tmp - float.Parse(textarea);
                case "×":
                    return tmp * float.Parse(textarea);
                case "÷":
                    // ゼロ除算の際の処理
                    if(float.Parse(textarea) == 0) return 0;
                    //ゼロ以外
                    else return tmp / float.Parse(textarea);
                default: 
                    return tmp;
            }
        }
    }
}