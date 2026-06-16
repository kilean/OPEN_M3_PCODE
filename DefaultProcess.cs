using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using OIG;


public static class DefaultProcess
{
    public static List<TProcess> LoadProcessList(this XDocument xmlDefaultProcess)
    {
        List<TProcess> ProcessList = new List<TProcess>();
        //取得所有XML的工序節點
        var processes = xmlDefaultProcess.Descendants("Process");
        foreach (var process in processes)
        {
            //新增一筆工序
            TProcess p = new TProcess();
            string ProcID = process.Attribute("ID")?.Value; //工序ID 內圓1~9, 外圓11~20, 量測201~203, 自訂程式999                                                                //例外處理
            if (string.IsNullOrEmpty(ProcID))
            {
                Fo_Msg.Show("[DefaultProcess.xml] Format Error : ID Empty", "Error.");
                return ProcessList;
            }
            int.TryParse(ProcID, out int id);
            if (id == 0)
            {
                Fo_Msg.Show("[DefaultProcess.xml] Format Error : ID Zero.", "Error.");
                return ProcessList;
            }
            p.ID = id;

            //新增一筆加工程式
            string ProgNo = process.Attribute("ProgNo")?.Value; //程式號 O9010~9018, O9020~O9029, O9702~O9704

            if (id != 999 && string.IsNullOrEmpty(ProgNo))
            {
                Fo_Msg.Show("[DefaultProcess.xml] Format Error : ProgNo Empty", "Error.");
                return ProcessList;
            }
            TSubProgram subPrograms = new TSubProgram();//固定一個
            int.TryParse(ProgNo, out int no);
            subPrograms.ProgNo = no;

            //加入P Code 資料
            foreach (var pcode in process.Elements("PCode"))
            {
                TArgument a = new TArgument();
                if (int.TryParse(pcode.Attribute("pages")?.Value, out int type)) a.Type = type; //參數1, 參數2, 參數3, 進階, 修整1, 修整2, 隱藏
                a.AddrCode = pcode.Attribute("PCode")?.Value; //P Code Macro
                if (double.TryParse(pcode.Attribute("Show")?.Value, out double show)) a.Value = show; //預設值
                if (double.TryParse(pcode.Attribute("Min")?.Value, out double min)) a.Min = min; // 最小值
                if (double.TryParse(pcode.Attribute("Max")?.Value, out double max)) a.Max = max; // 最大值
                a.Code = pcode.Attribute("Code")?.Value;//代碼
                a.Unit = pcode.Attribute("Unit")?.Value;//單位
                subPrograms.Arguments.Add(a);
            }

            p.SubPrograms.Add(subPrograms);

            ProcessList.Add(p);
        }
        return ProcessList;
    }
}







