using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace mission1
{
    class Program
    {
        static public void Header(string[] imformation2 , int[] imformation10 , char[] imformation16 ) // 0~~~~19 byte data 처리
        {
            string version = "";                                                                         // EDID 버전 표기 해야함
            string date = "";                                                                            // 제조 날짜 표기 해야함
            string Serialnumber = "";                                                                    // Serialnumber 표기 해야함
            string productname = "";                                                                     // 제품명 이름 표기 해야함
            string realname = "";                                                                        // 제조사 이름 표기 해야함
            string[] temporaryname = new string[3];

            string name = (imformation2[16] + imformation2[17] + imformation2[18] + imformation2[19]).Remove(0,1);

            for (int i = 0; name.Length > 0; i++)
            {
                temporaryname[i] = name.Substring(0, 5);
                name = name.Remove(0, 5);
                realname = realname + (char)(Convert.ToInt32(temporaryname[i], 2) + 64);
            }
       
            for (int j = 22; j >= 20; j -= 2)
                productname = productname + imformation16[j] + imformation16[j + 1];

            for (int k = 30; k >= 24; k -= 2)
                Serialnumber = Serialnumber + imformation16[k] + imformation16[k + 1];

            date = date + (1990 + ( imformation10[34] * 16 + imformation10[35] ) ) + "/"  +  imformation10[33] ;

            if (imformation10[39] == 4)
                version = "EDID Version 1.4";
            else if (imformation10[39] == 3)
                version = "EDID Version 1.3";
            else
                version = "unKnown";
            //////////표기
            /*
            Console.WriteLine($"Header Information");
            Console.WriteLine($"제조사 : {realname}");
            Console.WriteLine($"제품명 :  {productname}");
            Console.WriteLine($"일렬번호 : {Serialnumber}");
            Console.WriteLine($"제조 날짜 : {date}");
            Console.WriteLine($"{version}");*/
            //////////
        }

        static public void Basicdisplayparameters(string[] imformation2, int[] imformation10, char[] imformation16)
        {

            string input_type = "";                     // 입력 유형 Digital or Analog input
            string bit_depth = "";                      // bit per color(Digital)
            string Video_interface = "";                // Video_interface(Digital)
            string onoff = "";                          // Video white and sync levels, relative to blank(Analog)
            string[] analog = new string[5];            // analog 특성들 0 (Analog)
            string M_H_imagesize = "";                  // 표현 가능 image size (가로)
            string M_V_imagesize = "";                  // 표현 가능 image size (세로)
            string gamma = "";                          // gamma 전송 특성
            if (imformation2[40].Substring(0,1) == "1")   ////디지털 일때
            {
                input_type = "Digital input";
                switch(imformation10[40] - 8)
                {
                    case 0 :
                        bit_depth = "undefined";
                        break;
                    case 7:
                        bit_depth = "reserved";
                        break;
                    default:
                        bit_depth = ( 4 + ( 2 * (imformation10[40] - 8) ) ).ToString() + " bits per color ";
                        break;
                }

                switch (imformation10[41])
                {
                    case 0:
                        Video_interface = "undefined";
                        break;
                    case 2:
                        Video_interface = "HDMIa";
                        break;
                    case 3:
                        Video_interface = "HDMIb";
                        break;
                    case 4:
                        Video_interface = "MDDI";
                        break;
                    case 5:
                        Video_interface = "DisplayPort";
                        break;
                }
            }
            else if(imformation2[40].Substring(0, 1) == "0")  ///아날로그 일때 
            {
                input_type = "Analog input";
                switch ( Convert.ToInt32(imformation2[40].Remove(3, 1) ,2 )  )
                {
                    case 0 :
                        onoff = "+0.7V/-0.3V";
                        break;
                    case 1:
                        onoff = "+0.714V/-0.286V";
                        break;
                    case 2:
                        onoff = "+1.0V/-0.4V";
                        break;
                    case 3:
                        onoff = "+0.7V/-0V";
                        break;
                }
                if (imformation2[40].Substring(3, 1) == "1") analog[0] = "Blank-to-black setup (pedestal) expected";
                else analog[0] = "Blank-to-black none setup (pedestal) expected";
                if (imformation2[41].Substring(0, 1) == "1") analog[0] = "Separate sync supported";
                else analog[0] = "0";
                if (imformation2[40].Substring(1, 1) == "1") analog[0] = "1";
                else analog[0] = "0";
                if (imformation2[40].Substring(2, 1) == "1") analog[0] = "1";
                else analog[0] = "0";
                if (imformation2[40].Substring(3, 1) == "1") analog[0] = "1";
                else analog[0] = "0";

            }

            M_H_imagesize = (imformation10[42] * 10 + imformation10[43]).ToString();
            M_V_imagesize = (imformation10[44] * 10 + imformation10[45]).ToString();

            gamma = ( ( imformation10[46] * 16 + imformation10[47] + 100 ) / 100 ).ToString();
            Console.WriteLine($"{Convert.ToInt32(imformation2[40].Remove(3, 1), 2) }");
        }

        static void Main(string[] args)
        {
            char[] arr16 = new char[500];    // EDID 16 진수 저장
            int[] arr10 = new int[500];      // EDID 10 진수 저장
            string[] arr2 = new string[500]; // EDID 2 진수 저장
            using (StreamReader sr = new StreamReader(new FileStream(@args[0], FileMode.Open)))
            {
                 arr16 = sr.ReadToEnd().Replace(" ", sr.ReadToEnd()).ToCharArray();
            }
            for(int i = 0; i < arr16.Length ; i++)
            {
                arr10[i] = (int)arr16[i];
                if (arr10[i] <= 57) arr10[i] -= 48;
                else arr10[i] -= 55;
                arr2[i] = Convert.ToString(arr10[i], 2);
                for (int j = arr2[i].Length; j < 4; j++)
                    arr2[i] = "0" + arr2[i];
            }
            Header( arr2 ,arr10 , arr16 );
            Basicdisplayparameters(arr2, arr10, arr16);
            Console.WriteLine($"{arr10[34]}");
            Console.WriteLine($"{arr10[35]}");

        }
    }
}
