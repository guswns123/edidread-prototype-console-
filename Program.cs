using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace mission1
{
    static class Constants
    {
        
        public const int m = 36;
    }
    class Program
    {
        static public void Header(string[] imformation2 , int[] imformation10 , char[] imformation16 ) // 0~~~~19 byte data 처리
        {
            Console.WriteLine(imformation16.Length);
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
            
            Console.WriteLine($"Header Information");
            Console.WriteLine($"제조사 : {realname}");
            Console.WriteLine($"제품명 :  {productname}");
            Console.WriteLine($"일렬번호 : {Serialnumber}");
            Console.WriteLine($"제조 날짜 : {date}");
            Console.WriteLine($"{version}\n");
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

            gamma = ( (float)( imformation10[46] * 16 + imformation10[47] + 100 ) / 100 ).ToString();

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
             Console.WriteLine($"Horizontal screen size : {M_H_imagesize}");
             Console.WriteLine($"Vertical screen size : {M_V_imagesize}");
             Console.WriteLine($"Gamma : {gamma}");
             Console.WriteLine($"{DPMS}");
             Console.WriteLine($"Standard sRGB colour space : {Display_parameters[0]}");
             Console.WriteLine($"Preferred timing mode specified in descriptor block 1  : {Display_parameters[1]}");
             Console.WriteLine($"Continuous timings with GTF or CVT : {Display_parameters[2]}\n");
            
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
            
            Console.WriteLine($"Red x : {Red_xy[0]}");
            Console.WriteLine($"Red y : {Red_xy[1]}");
            Console.WriteLine($"Green x : {Green_xy[0]}");
            Console.WriteLine($"Green y : {Green_xy[1]}");
            Console.WriteLine($"Blue x : {Blue_xy[0]}");
            Console.WriteLine($"Blue y : {Blue_xy[1]}");
            Console.WriteLine($"White x : {White_xy[0]}");
            Console.WriteLine($"White y : {White_xy[1]}\n");
            
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
                    if (i == 0) timing_bitamp += "1152x870 ";
                    else if (i == 1) timing_bitamp += "1280x1024 ";
                    else if (i <= 5) timing_bitamp += "1024x768 ";
                    else if (i == 6) timing_bitamp += "832x624 ";
                    else if (i <= 10) timing_bitamp += "800x600 ";
                    else if (i <= 14) timing_bitamp += "640x480 ";
                    else if (i <= 16) timing_bitamp += "720x400 ";

                    if ((i == 4) || (i == 9) || (i == 14)) timing_bitamp = timing_bitamp + "@ 60Hz\n";
                    else if ((i == 0) || (i == 1) || (i == 2) || (i == 6) || (i == 7) || (i == 11)) timing_bitamp = timing_bitamp + "@ 75Hz\n";
                    else if ((i == 3) || (i == 16)) timing_bitamp = timing_bitamp + "@ 70Hz\n";
                    else if (i == 5) timing_bitamp = timing_bitamp + "@ 87Hz\n";
                    else if ((i == 8) || (i == 12)) timing_bitamp = timing_bitamp + "@ 72Hz\n";
                    else if (i == 13) timing_bitamp = timing_bitamp + "@ 67Hz\n";
                    else if (i == 15) timing_bitamp = timing_bitamp + "@ 88Hz\n";
                }

            }
            if (timing_bitamp == "")
                timing_bitamp = "none timing_bitmap";
            ////////////////////////////
            
            Console.WriteLine($"{timing_bitamp}\n");
            
            ////////////////////////////
        }

        static void Standard_Display_Modes(string[] imformation2, int[] imformation10, char[] imformation16)
        {
            string[] modes = new string[8];
            int[] X_resoloution = new int[8];
            int[] Y_resoloution = new int[8];
            int[] Vertical_frequency = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (imformation16[76 + (i * 4)].ToString() + imformation16[77 + (i * 4)].ToString() != "01")
                {
                    X_resoloution[i] = (imformation10[76 + (i * 2)] * 16 + imformation10[77 + (i * 2)] + 31) * 8; 
                    switch(imformation2[78 + (i * 4)].Substring(0,2))
                    {
                        case "00" :
                            Y_resoloution[i] = (X_resoloution[i] / 16) * 10;
                            modes[i] = "16:10";
                            break;
                        case "01":
                            Y_resoloution[i] = (X_resoloution[i] / 4) * 3;
                            modes[i] = "4:3";
                            break;
                        case "10":
                            Y_resoloution[i] = (X_resoloution[i] / 5) * 4;
                            modes[i] = "5:4";
                            break;
                        case "11":
                            Y_resoloution[i] = (X_resoloution[i] / 16) * 9;
                            modes[i] = "16:9";
                            break;

                    }

                    Vertical_frequency[i] = Convert.ToInt32(imformation2[78 + (i * 4)].Substring(2, 2), 2) * 16 + 
                        imformation10[79 + (i * 4)] + 60;
                    
                    Console.WriteLine($"X_resoloution : {X_resoloution[i]}");
                    Console.WriteLine($"Y-resoloution : {Y_resoloution[i]}");
                    Console.WriteLine($"Vertical_frequency : {Vertical_frequency[i]}");
                    Console.WriteLine($"modes : {modes[i]}\n");
                    
                }
            }

        }

        static void Description(string[] imformation2, int[] imformation10, char[] imformation16)
        {
            string[,] Description = new string[5, 20];
                for (int i = 0; i < ( (imformation16.Length - 112) / 36 ); i++)
                { 
                    if (imformation16[114 + (i * Constants.m)] == 'F' && (imformation10[108 + (i * Constants.m)] + imformation10[109 + (i * Constants.m)]
                        + imformation10[110 + (i * Constants.m)] + imformation10[111 + (i * Constants.m)] + imformation10[112 + (i * Constants.m)] + imformation10[113 + (i * Constants.m)]) == 0) /// 150 144~149
                    {
                        if (imformation10[115 + (i * Constants.m)] >= 12 && imformation10[115 + (i * Constants.m)] != 13) ////151
                        {
                            for (int j = 0; j <= 13; j++)
                            {
                                Description[i, 0] = Description[i, 0] + Convert.ToChar(imformation10[116 + (j * 2) + (i * Constants.m)] * 16 + imformation10[117 + (j * 2) + (i * Constants.m)]);//152 153
                            }
                            if (imformation16[115 + (i * Constants.m)] == 'F')////151
                                Description[i, 0] = "Display serial number :" + Description[i, 0];
                            else if (imformation16[115 + (i * Constants.m)] == 'E')
                                Description[i, 0] = "Unspecified text :" + Description[i, 0];
                            else if (imformation16[115 + (i * Constants.m)] == 'C')
                                Description[i, 0] = "Display name :" + Description[i, 0];
                        }
                        else if (imformation16[115 + (i * Constants.m)] == 'D')                                             //////////////////////////////EDID_Display_Range_Limits start 151
                        {
                            Description[i, 0] = "EDID_Display_Range_Limits";
                            if (imformation2[117 + (i * Constants.m)].Substring(0, 2) == "00")
                                Description[i, 1] = "Horizontal rate offsets : none";
                            else if (imformation2[117 + (i * Constants.m)].Substring(0, 2) == "10")   ///////153
                                Description[i, 1] = "Horizontal rate offsets : +255 kHz for max. rate";
                            else if (imformation2[117 + (i * Constants.m)].Substring(0, 2) == "11") //////153
                                Description[i, 1] = "Horizontal rate offsets : +255 kHz for max and min. rate";

                            Description[i, 2] = "Minimum vertical line rate : " + (imformation10[118 + (i * Constants.m)] * 16 + imformation10[119] + (i * Constants.m)).ToString() + "Hz";  ///154 155
                            Description[i, 3] = "Maximum vertical line rate : " + (imformation10[120 + (i * Constants.m)] * 16 + imformation10[121 + (i * Constants.m)]).ToString() + "Hz";  ////156 157
                            Description[i, 4] = "Minimum horizontal line rate : " + (imformation10[122 + (i * Constants.m)] * 16 + imformation10[123 + (i * Constants.m)]).ToString() + "Hz"; ////158 159
                            Description[i, 5] = "Maximum horizontall line rate : " + (imformation10[124 + (i * Constants.m)] * 16 + imformation10[125 + (i * Constants.m)]).ToString() + "Hz"; ///160 161
                            if (imformation16[129 + (i * Constants.m)] == '4') /////165
                                Description[i, 6] = "Maximum pixel clock rate : " + ((float)((imformation10[126 + (i * Constants.m)] * 16 + imformation10[127 + (i * Constants.m)]) * 10) +    ///////162 163 168 169 
                                                             (float)Convert.ToInt32(imformation2[132 + (i * Constants.m)] + imformation2[133 + (i * Constants.m)].Substring(0, 2), 2) * 0.25).ToString() + "MHz";
                            else
                                Description[i, 6] = "Maximum pixel clock rate : " + ((imformation10[126 + (i * Constants.m)] * 16 + imformation10[127 + (i * Constants.m)]) * 10).ToString() + "MHz";
                            if (imformation16[129 + (i * Constants.m)] == '0')
                                Description[i, 7] = "Extended timing information type : Default GTF";
                            else if (imformation16[129 + (i * Constants.m)] == '1')
                                Description[i, 7] = "Extended timing information type : No timing information.";
                            else if (imformation16[129 + (i * Constants.m)] == '2')                                 /////////////////////////With GTF secondary curve start
                            {
                                Description[i, 7] = "Extended timing information type : Secondary GTF supported, parameters as follows.";
                                Description[i, 8] = "start frequency : " + ((imformation10[132 + (i * Constants.m)] * 10 + imformation10[133 + (i * Constants.m)]) * 2).ToString() + "KHz";///////168   169
                                Description[i, 9] = "GTF C value : " + ((float)(imformation10[134 + (i * Constants.m)] * 10 + imformation10[135 + (i * Constants.m)]) / 2).ToString();     ///////171
                                Description[i, 10] = "GTF M value : " + (Convert.ToInt32(imformation2[138 + (i * Constants.m)] + imformation2[139 + (i * Constants.m)]                     ///////174~5 172 ~3
                                    + imformation2[136 + (i * Constants.m)] + imformation2[137 + (i * Constants.m)], 2)).ToString();
                                Description[i, 11] = "GTF K value : " + (imformation10[140 + (i * Constants.m)] * 10 + imformation10[141 + (i * Constants.m)]).ToString();                   ////////176~7
                                Description[i, 12] = "GTF J value : " + ((float)(imformation10[142 + (i * Constants.m)] * 10 + imformation10[143 + (i * Constants.m)]) / 2).ToString();      ////////178~9
                            }                                                                             //////////////////////////////////With GTF secondary curve end
                            else if (imformation16[129 + (i * Constants.m)] == '4')                                ////////////////////With CVT support start
                            {
                                Description[i, 7] = "Extended timing information type : CVT.";
                                Description[i, 8] = "CVT major version" + imformation10[130 + (i * Constants.m)].ToString(); /////////////166
                                Description[i, 9] = "CVT minor version" + imformation10[131 + (i * Constants.m)].ToString();/////////167
                                Description[i, 10] = "Maximum active pixels per line : " + Convert.ToInt32(imformation2[133 + (i * Constants.m)].Substring(2, 2) //////169
                                    + imformation2[170 + (i * Constants.m)] + imformation2[135 + (i * Constants.m)], 2).ToString();////171
                                Description[i, 11] = "Aspect ratio bitmap : ";
                                for (int k = 0; k < (imformation2[136 + (i * 34)] + imformation2[137 + (i * Constants.m)].Substring(0, 1)).Length; k++)//////172 173
                                {
                                    if (((imformation2[136 + (i * Constants.m)] + imformation2[137 + (i * Constants.m)].Substring(0, 1))).Substring(k, 1) == "1")///////172 173
                                    {
                                        switch (k)
                                        {
                                            case 0:
                                                Description[i, 11] += "4:3\n";
                                                break;
                                            case 1:
                                                Description[i, 11] += "16:9\n";
                                                break;
                                            case 2:
                                                Description[i, 11] += "16:10\n";
                                                break;
                                            case 3:
                                                Description[i, 11] += "5:4\n";
                                                break;
                                            case 4:
                                                Description[i, 11] += "15:9\n";
                                                break;
                                        }
                                    }
                                }
                                Description[i, 12] = "Aspect ratio preference";
                                switch ((Convert.ToInt32(imformation2[138 + (i * Constants.m)].Substring(0, 3), 2)))//////174
                                {
                                    case 0:
                                        Description[i, 12] = "4:3";
                                        break;
                                    case 1:
                                        Description[i, 12] = "16:9";
                                        break;
                                    case 2:
                                        Description[i, 12] = "16:10";
                                        break;
                                    case 3:
                                        Description[i, 12] = "5:4";
                                        break;
                                    case 4:
                                        Description[i, 12] = "15:9";
                                        break;
                                }
                                if (imformation10[138 + (i * Constants.m)] % 2 == 1)     ////174
                                    Description[i, 13] = "CVT-RB reduced blanking";
                                else
                                    Description[i, 13] = "CVT-RB none reduced blanking";
                                if (imformation10[139 + (i * Constants.m)] >= 8)////175
                                    Description[i, 14] = "CVT standard blanking";
                                else
                                    Description[i, 14] = "CVT standard blanking = none";
                                Description[i, 15] = "Horizontal shrink";
                                Description[i, 16] = "Horizontal stretch";
                                Description[i, 17] = "Vertical shrink";
                                Description[i, 18] = "Vertical stretch";
                                for (int k = 0; k < 4; k++)
                                {
                                    if (imformation2[140 + (i * Constants.m)].Substring(k, 1) == "1")///176
                                        Description[i, 15 + k] = "none" + Description[i, 15 + k];
                                }
                                Description[i, 20] = (imformation10[142 + (i * Constants.m)] * 16 + imformation10[143 + (i * Constants.m)]).ToString();////178 179

                            }                                                                            /////////////////////////////With CVT support end
                        }                                                                               ///////////////////////////////EDID_Display_Range_Limits end
                        else if (imformation16[114 + (i * Constants.m)] == 'B')           ///150                         ///////////////////////////////Additional white point descriptor start
                        {
                            Description[i, 0] = "Additional white point descriptor";
                            Description[i, 1] = "white point index number : " + (imformation10[118 + (i * Constants.m)] * 16 + imformation10[119 + (i * Constants.m)]).ToString();///154 155
                            Description[i, 2] = "White point x : " + (((float)Convert.ToInt32(imformation2[122 + (i * Constants.m)] +//////158
                                imformation2[123 + (i * Constants.m)] + imformation2[121 + (i * Constants.m)].Substring(0, 2), 2)) / 1024).ToString();///159 157
                            Description[i, 3] = "White point y : " + (((float)Convert.ToInt32(imformation2[124 + (i * Constants.m)] + ///160   
                            imformation2[125 + (i * Constants.m)] + imformation2[122 + (i * Constants.m)].Substring(2, 2), 2)) / 1024).ToString();//161 157
                            Description[i, 4] = "gamma : " + ((float)(imformation10[126 + (i * Constants.m)] * 16 + imformation10[127 + (i * Constants.m)] + 100) / 100).ToString();///162 163
                        }                                                                                 ///////////////////////////////Additional white point descriptor end
                        else if (imformation16[114 + (i * Constants.m)] == '9')//150
                        {
                            Description[i, 0] = "Color management data descriptor";
                            Description[i, 1] = "Version : 03";
                            Description[i, 2] = "Red a3";
                            Description[i, 3] = "Red a2";
                            Description[i, 4] = "Green a3";
                            Description[i, 5] = "Greeen a2";
                            Description[i, 6] = "Blue a3";
                            Description[i, 7] = "Blue a2";
                            for (int m = 0; m < 8; m++)
                            {
                                Description[i, 2 + m] += imformation16[120 + (m * 4) + (i * Constants.m)].ToString() + imformation16[121 + (m * 4) + (i * Constants.m)].ToString() //156~7
                                    + imformation16[118 + (m * 4) + (i * Constants.m)].ToString() + imformation16[119 + (m * 4) + (i * Constants.m)].ToString();//154~5
                            }


                        }
                        else if (imformation10[114 + (i * Constants.m)] == '8')                                   /////////////////////////////////EDID CVT 3-byte timing codes descriptor start
                        {
                            Description[i, 0] = "Color management data descriptor";
                            Description[i, 1] = "Verson : 01";
                            for (int l = 0; l < 4; l++)
                            {
                                Description[i, 2 + (l * 4)] = "Addressable lines" + Convert.ToInt32(imformation2[122 + (i * Constants.m)] + imformation2[120 + (i * Constants.m)] + imformation2[121 + (i * Constants.m)]).ToString();///158 156 157
                                Description[i, 3 + (l * 4)] = "Aspect ratio : ";
                                if (imformation10[123 + (i * Constants.m)] == 0) Description[i, 3 + (l * 4)] += "4 : 3";/////////159
                                else if (imformation10[123 + (i * Constants.m)] == 4) Description[i, 3 + (l * 4)] += "16 : 9";
                                else if (imformation10[123 + (i * Constants.m)] == 8) Description[i, 3 + (l * 4)] += "16 : 10";
                                else if (imformation10[123 + (i * Constants.m)] == 12) Description[i, 3 + (l * 4)] += "15 : 9";////////159
                                Description[i, 4 + (l * 4)] = "Preferred vertical rate";
                                if (imformation2[124 + (i * Constants.m)].Substring(1, 2) == "00") Description[i, 4 + (l * 4)] += "50Hz"; //////160
                                else if (imformation2[124 + (i * Constants.m)].Substring(1, 2) == "01") Description[i, 4 + (l * 4)] += "60Hz";
                                else if (imformation2[124 + (i * Constants.m)].Substring(1, 2) == "10") Description[i, 4 + (l * 4)] += "75Hz";
                                else if (imformation2[124 + (i * Constants.m)].Substring(1, 2) == "00") Description[i, 4 + (l * 4)] += "85Hz";//////160
                                Description[i, 5 + (l * 4)] = "Vertical rate bitmap";
                                string ex = imformation2[124 + (i * Constants.m)].Substring(3, 1) + imformation2[125 + (i * Constants.m)];
                                for (int k = 0; k < ex.Length; k++)//160 161
                                {
                                    if (ex.Substring(k, 1) == "1")//160 161
                                    {
                                        switch (k)
                                        {
                                            case 0:
                                                Description[i, 5 + (l * 4)] += "50Hz\n";
                                                break;
                                            case 1:
                                                Description[i, 5 + (l * 4)] += "60Hz\n";
                                                break;
                                            case 2:
                                                Description[i, 5 + (l * 4)] += "75Hz\n";
                                                break;
                                            case 3:
                                                Description[i, 5 + (l * 4)] += "85Hz\n";
                                                break;
                                            case 4:
                                                Description[i, 5 + (l * 4)] += "60Hz reduced blanking\n";
                                                break;
                                        }
                                    }
                                }
                            }


                        }                                                                                          //////////////////////////////EDID CVT 3-byte timing codes descriptor end
                        else if (imformation10[114 + (i * Constants.m)] == '7')///150
                        {
                            string sumstring2 = "";
                            Description[i, 0] = "Additional standard timings";
                            Description[i, 1] = "version : 10";
                            for (int k = 0; k < 12; k++)
                                sumstring2 += imformation2[120 + (i * Constants.m) + k];//// 156

                            for (int n = 0; n < sumstring2.Length; n++)
                            {
                                if (sumstring2.Substring(n, 1) == "1")
                                {
                                    if (n == 0) Description[i, 2] = "1152 X 864";
                                    else if (n == 1) Description[i, 2] = "1024 X 768";
                                    else if (n == 2) Description[i, 2] = "800 X 600";
                                    else if (n == 3) Description[i, 2] = "848 X 480";
                                    else if (n == 4) Description[i, 2] = "640 X 480";
                                    else if (n == 5) Description[i, 2] = "720 X 400";
                                    else if (n == 6) Description[i, 2] = "640 X 400";
                                    else if (n == 7) Description[i, 2] = "640 X 350";
                                    else if (n <= 11) Description[i, 2] = "1280 X 768";
                                    else if (n <= 13) Description[i, 2] = "1280 X 960";
                                    else if (n <= 15) Description[i, 2] = "1280 X 1024";
                                    else if (n == 16) Description[i, 2] = "1360 X 768";
                                    else if (n == 17) Description[i, 2] = "1280 X 768";
                                    else if (n <= 20) Description[i, 2] = "1440 X 900";
                                    else if (n <= 24) Description[i, 2] = "1440 X 1050";
                                    else if (n <= 28) Description[i, 2] = "1680 X 1050";
                                    else if (n <= 33) Description[i, 2] = "1680 X 1200";
                                    else if (n <= 35) Description[i, 2] = "1792 X 1344";
                                    else if (n <= 37) Description[i, 2] = "1856 X 1392";
                                    else if (n <= 41) Description[i, 2] = "1920 X 1200";
                                    else if (n <= 43) Description[i, 2] = "1920 X 1440";

                                    if (n <= 7 || n != 4 || n == 11 || n == 13 || n == 15 || n == 20 || n == 24 || n == 28 || n == 33 || n == 42) Description[i, 2] += "85Hz";
                                    else if (n == 4 || n == (8) || n == 9 || n == 12 || n == 14 || n == (16) || n == 17 || n == (18) ||
                                                n == (21) || n == 22 || n == (24) || n == 25 || n == (26) || n == (29) || n == 34 || n == 36 || n == (38) || n == 39 || n == 42) Description[i, 2] += "60Hz";
                                    else if (n == 30) Description[i, 2] += "65Hz";
                                    else Description[i, 2] += "75Hz";
                                    if (n == 8 || n == 16 || n == 18 || n == 21 || n == 24 || n == 26 || n == 29 || n == 38) Description[i, 2] += " (CVT - RB)";

                                }
                            }
                        }
                    }
                    else if (imformation16[114 + (i * Constants.m)] == '0' && (imformation10[108 + (i * Constants.m)] + imformation10[109 + (i * Constants.m)]  ////////150 144~9
                        + imformation10[110 + (i * Constants.m)] + imformation10[111 + (i * Constants.m)] + imformation10[112 + (i * Constants.m)] + imformation10[113 + (i * Constants.m)]) == 0)
                    {
                        Console.WriteLine("Manufacturer reserved descriptors.");
                    }
                    else
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
                        string stereo = "";
                        string sync_flag = "";
                        string[] Analog_sync = new string[3];               //0 : Sync type 1 : Serration 2 : Sync on red and blue lines additionally to green
                        string[] Digital_sync = new string[2];              /*composite mode(0 : Serration 1 : Horizontal sync polarity) 
                                                                  separate mode (0 : Vertical sync polarity 1 : Horizontal sync polarity)*/

                        Pixel_clock = ((float)(Convert.ToInt32(imformation2[110 + (i * Constants.m)] + imformation2[111 + (i * Constants.m)] + imformation2[108 + (i * Constants.m)] + imformation2[109 + (i * Constants.m)], 2) * 10000)
                            / 1000000).ToString() + "MHz";
                        if (imformation10[110 + (i * Constants.m)] + imformation10[111 + (i * Constants.m)] + imformation10[108 + (i * Constants.m)] + imformation10[109 + (i * Constants.m)] == 0)
                            Pixel_clock = "10KHz";

                        Horizontal_Active = (imformation10[112 + (i * Constants.m)] * 16 + imformation10[113 + (i * Constants.m)]) + imformation10[116 + (i * Constants.m)] * 256;
                        Horizontal_Blanking = (imformation10[114 + (i * Constants.m)] * 16 + imformation10[115 + (i * Constants.m)]) + imformation10[117 + (i * Constants.m)] * 256;

                        Vertical_Active = (imformation10[118 + (i * Constants.m)] * 16 + imformation10[119 + (i * Constants.m)]) + imformation10[122 + (i * Constants.m)] * 256;
                        Vertical_Blanking = (imformation10[120 + (i * Constants.m)] * 16 + imformation10[121 + (i * Constants.m)]) + imformation10[123 + (i * Constants.m)] * 256;

                        Horizontal_Sync_Offset = (imformation10[124 + (i * Constants.m)] * 16 + imformation10[125 + (i * Constants.m)]) + Convert.ToInt32(imformation2[130 + (i * Constants.m)].Substring(0, 2), 2) * 256;
                        Horizontal_Sync_Pulse = (imformation10[126 + (i * Constants.m)] * 16 + imformation10[127 + (i * Constants.m)]) + Convert.ToInt32(imformation2[130 + (i * Constants.m)].Substring(2, 2), 2) * 256;

                        Vertical_Sync_Offset = imformation10[128 + (i * Constants.m)] + Convert.ToInt32(imformation2[131 + (i * Constants.m)].Substring(0, 2), 2) * 16;
                        Vertical_Sync_Pulse = imformation10[129 + (i * Constants.m)] + Convert.ToInt32(imformation2[130 + (i * Constants.m)].Substring(2, 2), 2) * 16;

                        Horizontal_Display_Size = (imformation10[132 + (i * Constants.m)] * 16 + imformation10[133 + (i * Constants.m)]) + imformation10[136 + (i * Constants.m)] * 256;
                        Vertical_Display_Size = (imformation10[134 + (i * Constants.m)] * 16 + imformation10[135 + (i * Constants.m)]) + imformation10[137 + (i * Constants.m)] * 256;

                        Horizontal_Border = (imformation10[138 + (i * Constants.m)] * 16) + imformation10[139 + (i * Constants.m)];
                        Vertical_Border = (imformation10[140 + (i * Constants.m)] * 16) + imformation10[141 + (i * Constants.m)];

                        if (imformation10[142 + (i * Constants.m)] >= 8)
                            interlace = "interlaced";
                        else
                            interlace = "non-inerlaced";

                        if (Convert.ToInt32(imformation2[142 + (i * Constants.m)].Substring(1, 2), 2) == 0)
                            stereo = "X";
                        else if (Convert.ToInt32(imformation2[142 + (i * Constants.m)].Substring(1, 2), 2) == 1)
                        {
                            if (imformation10[143 + (i * Constants.m)] % 2 == 1)
                                stereo = "field sequential, right during stereo sync";
                            else
                                stereo = "2-way interleaved, right image on even lines";
                        }
                        else if (Convert.ToInt32(imformation2[142 + (i * Constants.m)].Substring(1, 2), 2) == 2)
                        {
                            if (imformation10[143 + (i * Constants.m)] % 2 == 1)
                                stereo = "field sequential, left during stereo sync";
                            else
                                stereo = "2-way interleaved, left image on even lines";
                        }
                        else if (Convert.ToInt32(imformation2[142 + (i * Constants.m)].Substring(1, 2), 2) == 3)
                        {
                            if (imformation10[143 + (i * Constants.m)] % 2 == 1)
                                stereo = "4-way interleaved";
                            else
                                stereo = "side-by-side interleaved";
                        }

                        if (imformation10[142 + (i * Constants.m)] % 2 == 0)
                        {
                            sync_flag = "analog";
                            if (imformation2[143 + (i * Constants.m)].Substring(1, 0) == "1")
                                Analog_sync[0] = "Sync type : bipolar analog composite.";
                            else
                                Analog_sync[0] = "Sync type : analog composite.";
                            if (imformation2[143 + (i * Constants.m)].Substring(2, 0) == "1")
                                Analog_sync[1] = "Serration : with serrations (H-sync during V-sync).";
                            else
                                Analog_sync[1] = "Serration : without serrations.";
                            if (imformation2[143 + (i * Constants.m)].Substring(2, 0) == "1")
                                Analog_sync[2] = "Sync on red and blue lines additionally to green : sync on all three (RGB) video signals.";
                            else
                                Analog_sync[2] = "Sync on red and blue lines additionally to green : sync on green signal only.";
                        }
                        else
                        {
                            sync_flag = "digital";
                            if (imformation10[143 + (i * Constants.m)] >= 8)
                            {
                                if (imformation2[143 + (i * Constants.m)].Substring(1, 1) == "1")
                                    Digital_sync[0] = "Vertical sync polarity : positive";
                                else
                                    Digital_sync[0] = "Vertical sync polarity : negative";
                            }
                            else
                            {
                                if (imformation2[143 + (i * Constants.m)].Substring(1, 1) == "1")
                                    Digital_sync[0] = "Serration : without serration;";
                                else
                                    Digital_sync[0] = "Serration : with serration (H-sync during V-sync)";
                            }

                            if (imformation2[143 + (i * Constants.m)].Substring(2, 1) == "1")
                                Digital_sync[1] = "Horizontal sync polarity : positive";
                            else
                                Digital_sync[1] = "Horizontal sync polarity : negative";

                        }

                        Console.WriteLine($"Pixel_clock : {Pixel_clock}");
                        Console.WriteLine($"Horizontal_Active : {Horizontal_Active}");
                        Console.WriteLine($"Horizontal_Blanking : {Horizontal_Blanking}");
                        Console.WriteLine($"Horizontal_Sync_Offset : {Horizontal_Sync_Offset}");
                        Console.WriteLine($"Horizontal_Sync_Pulse : {Horizontal_Sync_Pulse}");
                        Console.WriteLine($"Horizontal image size {Horizontal_Display_Size}");
                        Console.WriteLine($"Horizontal_Border : {Horizontal_Border}");
                        Console.WriteLine($"Vertical_Active : {Vertical_Active}");
                        Console.WriteLine($"Vertical_Blanking : {Vertical_Blanking}");
                        Console.WriteLine($"Vertical_Sync_Offset : {Vertical_Sync_Offset}");
                        Console.WriteLine($"Vertical_Sync_Pulse : {Vertical_Sync_Pulse}");
                        Console.WriteLine($"Vertical image size {Vertical_Display_Size}");
                        Console.WriteLine($"Vertical_Border : {Vertical_Border}");
                        Console.WriteLine($"interface mode : {interlace}");
                        Console.WriteLine($"stereo mode : {stereo}");
                        Console.WriteLine($"{sync_flag} syns");
                        if (sync_flag == "analog")
                            foreach (string j in Analog_sync)
                                Console.WriteLine($"{j}");
                        if (sync_flag == "digital")
                            foreach (string j in Digital_sync)
                                Console.WriteLine($"{j}");
                        Console.WriteLine("");
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 20; j++)
                        if (Description[i, j] != null)
                            Console.WriteLine(Description[i, j]);
                    Console.WriteLine("");
                }
        }



        static void Main(string[] args)
        {
            char[] arr16 = new char[1000];    // EDID 16 진수 저장
            int[] arr10 = new int[1000];      // EDID 10 진수 저장
            string[] arr2 = new string[1000]; // EDID 2 진수 저장
            string arr = "";
            using (StreamReader sr = new StreamReader(new FileStream(@args[0], FileMode.Open)))
            {
                 arr = (sr.ReadToEnd().Replace(Environment.NewLine, sr.ReadToEnd())).ToUpper();
                 arr16 = arr.Replace(" ", "").ToCharArray();
                
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
            Description(arr2, arr10, arr16);

        }
    }
}
