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

        static public void Basic_display_parameters(string[] imformation2, int[] imformation10, char[] imformation16)
        {
            string[] Display_parameters = new string[3];/* 0 : Standard sRGB colour space 1 : Preferred timing mode specified in descriptor block 1 
                                                           2 : Continuous timings with GTF or CVT*/
            string Display_type = "RGB 4:4:4";          // Display type
            string DPMS = "";                           // DPMS type
            string input_type = "";                     // 입력 유형 Digital or Analog input
            string bit_depth = "";                      // bit per color(Digital)
            string Video_interface = "";                // Video_interface(Digital)
            string onoff = "";                          // Video white and sync levels, relative to blank(Analog)
            string[] analog = new string[5];            /* 0 : Blank-to-black setup 1 : Separate sync supported 2 : Composite sync(on HSync) supported
                                                           3 : Sync on green supported 4 : VSync pulse must be serrated when composite or sync-on - green is used.(Analog)*/
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
                if (imformation2[49].Substring(0,1) == "1")
                {
                    Display_type = Display_type + "+ YCrCb 4:4:4";
                    if (imformation2[48].Substring(3, 1) == "1")
                        Display_type = Display_type + "+ YCrCb 4:2:2";
                }
                else
                {
                    if (imformation2[48].Substring(3, 1) == "1")
                        Display_type = Display_type + "+ YCrCb 4:2:2";
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
                if (imformation2[41].Substring(0, 1) == "1") analog[1] = "Separate sync supported";
                else analog[1] = "Separate sync none supported";
                if (imformation2[40].Substring(1, 1) == "1") analog[2] = "Composite sync (on HSync) supported";
                else analog[2] = "Composite sync (on HSync) none supported";
                if (imformation2[40].Substring(2, 1) == "1") analog[3] = "Sync on green supported";
                else analog[3] = "Sync on green none supported";
                if (imformation2[40].Substring(3, 1) == "1") analog[4] = "VSync pulse must be serrated when composite or sync-on-green is used.";
                else analog[4] = "VSync pulse must be not serrated when composite or sync-on-green is used.";

                switch(Convert.ToInt32(imformation2[48].Substring(3, 1), 2) + Convert.ToInt32(imformation2[49].Substring(0, 1), 2))
                {
                    case 0:
                        Display_type = "monochrome or grayscale";
                        break;
                    case 1:
                        Display_type = "RGB color";
                        if (imformation2[48].Substring(3, 1) == "1")
                            Display_type = "none" + Display_type;
                        break;
                    case 2:
                        Display_type = "undefined";
                        break;

                }

            }

            M_H_imagesize = (imformation10[42] * 10 + imformation10[43]).ToString();
            M_V_imagesize = (imformation10[44] * 10 + imformation10[45]).ToString();

            gamma = ( ( imformation10[46] * 16 + imformation10[47] + 100 ) / 100 ).ToString();

            if (imformation2[48].Substring(3, 1) == "1")
                DPMS = "DPMS standby supported";
            else if (imformation2[48].Substring(2, 1) == "1")
                DPMS = "DPMS suspend supported";
            else if (imformation2[48].Substring(1, 1) == "1")
                DPMS = "DPMS active-off supported";
            else
                DPMS = "none DPMS";

            for( int i = 1;  i  <=  3; i++ )
            {
                if (imformation2[49].Substring(i, 1) == "1")
                    Display_parameters[i - 1] = "yes";
                else
                    Display_parameters[i - 1] = "no";
            }
    

            ///////////////////////////////////
            /*
             if(input_type == "Digital input")
             {
                 Console.WriteLine($"{input_type}");
                 Console.WriteLine($"bit_depth = {bit_depth}");
                 Console.WriteLine($"Video_interface = {Video_interface}");
             }
             else if (input_type == "Analog input")
             {
                 Console.WriteLine($"{input_type}");
                 Console.WriteLine($"on off = {onoff}");
                 for( int i = 0; i < analog.Length; i++ )
                     Console.WriteLine($"{analog[i]}");
             }          
             Console.WriteLine($"{ Display_type }");
             Console.WriteLine($"{DPMS}");
             Console.WriteLine($"Standard sRGB colour space : {Display_parameters[0]}");
             Console.WriteLine($"Preferred timing mode specified in descriptor block 1  : {Display_parameters[1]}");
             Console.WriteLine($"Continuous timings with GTF or CVT : {Display_parameters[2]}");
            */
            ///////////////////////////////////
            ///
        }

        static void Chromaticity_coordinates (string[] imformation2, int[] imformation10, char[] imformation16)
        {
            float[] Red_xy = new float[2];
            float[] Green_xy = new float[2];
            float[] Blue_xy = new float[2];
            float[] White_xy = new float[2];
            for (int i = 0; i < 2; i++)
            {
                Red_xy[i] = ((float)Convert.ToInt32(imformation2[50].Substring(i * 2, 2), 2) / 1024) +
                    ((float)Convert.ToInt32(imformation2[54 + (i * 2)] + imformation2[55 + (i * 2)], 2) / 256);

                Green_xy[i] = ((float)Convert.ToInt32(imformation2[51].Substring(i * 2, 2), 2) / 1024) +
                    ((float)Convert.ToInt32(imformation2[58 + (i * 2)] + imformation2[59 + (i * 2)], 2) / 256);

                Blue_xy[i] = ((float)Convert.ToInt32(imformation2[52].Substring(i * 2 , 2), 2) / 1024) +
                    ((float)Convert.ToInt32(imformation2[62 + (i * 2)] + imformation2[63 + (i * 2)], 2) / 256);

                White_xy[i] = ((float)Convert.ToInt32(imformation2[53].Substring(i * 2, 2), 2) / 1024) +
                    ((float)Convert.ToInt32(imformation2[66 + (i * 2)] + imformation2[67 + (i * 2)], 2) / 256);

            }

            //////////////////////
            /*
            Console.WriteLine($"{Red_xy[0]}");
            Console.WriteLine($"{Red_xy[1]}");
            Console.WriteLine($"{Green_xy[0]}");
            Console.WriteLine($"{Green_xy[1]}");
            Console.WriteLine($"{Blue_xy[0]}");
            Console.WriteLine($"{Blue_xy[1]}");
            Console.WriteLine($"{White_xy[0]}");
            Console.WriteLine($"{White_xy[1]}");
            */
            //////////////////////
        }

        static void Established_timing_bitmap(string[] imformation2, int[] imformation10, char[] imformation16)
        {
            string timing_bitamp = "";
            string sumstring = imformation2[70] + imformation2[71] + imformation2[72] + imformation2[73] + imformation2[74].Substring(0,1);
            if (Convert.ToInt32(imformation2[74].Substring(1,3),2) + imformation10[75] > 0)
                timing_bitamp = "Other manufacturer-specific display modes";
            for (int i = 0; i < sumstring.Length; i++)
            { 
                if (sumstring.Substring((sumstring.Length - 1) - i, 1) == "1")
                {
                    if (i == 0) timing_bitamp = "1152x870 ";
                    else if (i == 1) timing_bitamp = "1280x1024 ";
                    else if (i <= 5) timing_bitamp = "1024x768 ";
                    else if (i == 6) timing_bitamp = "832x624 ";
                    else if (i <= 10) timing_bitamp = "800x600 ";
                    else if (i <= 14) timing_bitamp = "640x480 ";
                    else if (i <= 16) timing_bitamp = "720x400 ";

                    if ((i == 4) || (i == 9) || (i == 14)) timing_bitamp = timing_bitamp + "@ 60Hz";
                    else if ((i == 0) || (i == 1) || (i == 2) || (i == 6) || (i == 7) || (i == 11)) timing_bitamp = timing_bitamp + "@ 75Hz";
                    else if ((i == 3) || (i == 16)) timing_bitamp = timing_bitamp + "@ 70Hz";
                    else if (i == 5) timing_bitamp = timing_bitamp + "@ 87Hz";
                    else if ((i == 8) || (i == 12)) timing_bitamp = timing_bitamp + "@ 72Hz";
                    else if (i == 13) timing_bitamp = timing_bitamp + "@ 67Hz";
                    else if (i == 15) timing_bitamp = timing_bitamp + "@ 88Hz";
                }

            }
            ////////////////////////////
            /*
            Console.WriteLine($"{timing_bitamp}");
            */
            ////////////////////////////
        }

        static void Standard_Display_Modes(string[] imformation2, int[] imformation10, char[] imformation16)
        {
            string[] modes = new string[8];
            int[] X_resoloution = new int[8];
            int[] Vertical_frequency = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (imformation16[76 + (i * 4)].ToString() + imformation16[77 + (i * 4)].ToString() != "01")
                {
                    X_resoloution[i] = (imformation10[76 + (i * 2)] * 16 + imformation10[77 + (i * 2)] + 31) * 8; 
                    switch(imformation2[78 + (i * 4)].Substring(0,2))
                    {
                        case "00" :
                            modes[i] = "16:10";
                            break;
                        case "01":
                            modes[i] = "4:3";
                            break;
                        case "10":
                            modes[i] = "5:4";
                            break;
                        case "11":
                            modes[i] = "16:9";
                            break;

                    }
                    Vertical_frequency[i] = Convert.ToInt32(imformation2[78 + (i * 4)].Substring(2, 2), 2) * 16 + 
                        imformation10[79 + (i * 4)] + 60;
                    /*
                    Console.WriteLine($"{X_resoloution[i]}");
                    Console.WriteLine($"{Vertical_frequency[i]}");
                    Console.WriteLine($"{modes[i]}");
                    */
                }
            }

        }

        static void Description1(string[] imformation2, int[] imformation10, char[] imformation16)
        {

            string Pixel_clock = "";
            int Horizontal_Active = 0;
            int Horizontal_Blanking = 0;
            int Vertical_Active = 0;
            int Vertical_Blanking = 0;
            int Horizontal_Sync_Offset = 0;
            int Horizontal_Sync_Pulse = 0;
            int Vertical_Sync_Offset = 0;
            int Vertical_Sync_Pulse = 0;
            int Horizontal_Display_Size = 0;
            int Vertical_Display_Size = 0;
            int Horizontal_Border = 0;
            int Vertical_Border = 0;
            string interlace = "";


            Pixel_clock = ( (float)( Convert.ToInt32(imformation2[110] + imformation2[111] + imformation2[108] + imformation2[109], 2) * 10000) 
                / 1000000 ).ToString() + "MHz" ;
            if (imformation10[110] + imformation10[111] + imformation10[108] + imformation10[109] == 0)
                Pixel_clock = "10KHz";

            Horizontal_Active = (imformation10[112] * 16 + imformation10[113]) + imformation10[116] * 256;
            Horizontal_Blanking = (imformation10[114] * 16 + imformation10[115]) + imformation10[117] * 256;

            Vertical_Active = (imformation10[118] * 16 + imformation10[119]) + imformation10[122] * 256;
            Vertical_Blanking = (imformation10[120] * 16 + imformation10[121]) + imformation10[123] * 256;

            Horizontal_Sync_Offset = (imformation10[124] * 16 + imformation10[125]) + Convert.ToInt32(imformation2[130].Substring(0,2),2 ) * 256;
            Horizontal_Sync_Pulse = (imformation10[126] * 16 + imformation10[127]) + Convert.ToInt32(imformation2[130].Substring(2, 2), 2) * 256;

            Vertical_Sync_Offset = imformation10[128] + Convert.ToInt32(imformation2[131].Substring(0, 2), 2) * 16;
            Vertical_Sync_Pulse = imformation10[129] + Convert.ToInt32(imformation2[130].Substring(2, 2), 2) * 16;

            Horizontal_Display_Size = (imformation10[132] * 16 + imformation10[133]) + imformation10[136] * 256;
            Vertical_Display_Size = (imformation10[134] * 16 + imformation10[135]) + imformation10[137] * 256;

            Horizontal_Border = (imformation10[138] * 16) + imformation10[139];
            Vertical_Border = (imformation10[140] * 16) + imformation10[141];

            if (imformation10[142] >= 8)
                interlace = "interlaced";
            else
                interlace = "non-inerlaced";



            Console.WriteLine($"{Horizontal_Border}");
            Console.WriteLine($"{Vertical_Border}");

            /*       
            Console.WriteLine($"Pixel_clock : {Pixel_clock}");
            Console.WriteLine($"Horizontal_Active : {Horizontal_Active}");
            Console.WriteLine($"Horizontal_Blanking : {Horizontal_Blanking}");
            Console.WriteLine($"Vertical_Active : {Vertical_Active}");
            Console.WriteLine($"Vertical_Blanking : {Vertical_Blanking}");
            Console.WriteLine($"Horizontal_Sync_Offset : {Horizontal_Sync_Offset}");
            Console.WriteLine($"Horizontal_Sync_Pulse : {Horizontal_Sync_Pulse}");
            Console.WriteLine($"Vertical_Sync_Offset : {Vertical_Sync_Offset}");
            Console.WriteLine($"Vertical_Sync_Pulse : {Vertical_Sync_Pulse}");
            Console.WriteLine($"Horizontal_Border : {Horizontal_Border}");
            Console.WriteLine($"Vertical_Border : {Vertical_Border}");
            */
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
            Basic_display_parameters(arr2, arr10, arr16);
            Chromaticity_coordinates(arr2, arr10, arr16);
            Established_timing_bitmap(arr2, arr10, arr16);
            Standard_Display_Modes(arr2, arr10, arr16);
            Description1(arr2, arr10, arr16);
            //Console.WriteLine($"{arr2[49]}");
            //Console.WriteLine($"{arr10[35]}");

        }
    }
}
