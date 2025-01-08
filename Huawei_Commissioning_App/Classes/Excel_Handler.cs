using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // For .xlsx files
using NPOI.HSSF.UserModel; // For .xls files (older format)
using System;
using System.IO;
using System.Reflection.Metadata;


public class Excel
{
    TextEditor T = new TextEditor();
    // Path to the Excel file
    private static string[] PlanIPs_filesPath =
         {  @"IPs\1st half 2024_TE_IP_PLAN(2024-08-13 07_04_10).xlsx",
            @"IPs\1st half 2024_TED_ACC_Data(2024-08-13 07_04_26).xlsx"};

    private static string Input_Cabinet_Code_Path = @"Inputs\CAbinet_Sheet.xlsx";

    public Excel()
    {
        // Get the full path of the Excel file from the relative path.
        PlanIPs_filesPath[0] = Path.GetFullPath(PlanIPs_filesPath[0]);
        PlanIPs_filesPath[1] = Path.GetFullPath(PlanIPs_filesPath[1]);
        Input_Cabinet_Code_Path = Path.GetFullPath(Input_Cabinet_Code_Path);
    }

    public static bool Read_Cabinet_Sheet(Queue<Queue_Node> info)
    {
        Queue_Node queue_Node = new Queue_Node();
        bool Sheet_status = false;
        int rowIndex = 1; // start from row 1, as row 0 is the header.
        using (FileStream file = new FileStream(Input_Cabinet_Code_Path, FileMode.Open, FileAccess.Read))
        {
            IWorkbook? workbook = null;
            workbook = new XSSFWorkbook(file); // For .xlsx files                 
            if (workbook != null)
            {
                // Access the first worksheet
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row = sheet.GetRow(rowIndex);
                ICell? cell;
                // Loop through rows.
                while (row.GetCell(0) != null)
                {
                    if (row != null)
                    {
                        cell = row.GetCell(0); // Get the first cell, Family Name.
                        queue_Node.Cabinet_Family_Name = cell.ToString(); // Family Name.
                        cell = row.GetCell(1); // Get the second cell, Cabinet Type.
                        if (cell == null)
                            continue;
                        else
                        {
                            queue_Node.Cabinet_Type = cell.ToString(); // Cabinet Type.
                            cell = row.GetCell(2); // Get the third cell, Cabinet Code 1.
                            if (cell == null)
                                continue;
                            else
                            {
                                queue_Node.Code_1 = cell.ToString();// cabinet code 1.
                                if (queue_Node.Code_1 == null)
                                {
                                    Console.WriteLine("Code 2 is not existing.");
                                    continue;
                                }
                                while (queue_Node.Code_1[0] == ' ')
                                {
                                    queue_Node.Code_1 = queue_Node.Code_1.Substring(1);
                                }
                                // test code.
                                if (queue_Node.Cabinet_Type == "MA5818")
                                {
                                    // Get the fourth cell, Cabinet Code 2.
                                    cell = row.GetCell(3);
                                    if (cell == null)
                                        continue;
                                    else
                                    {
                                        queue_Node.Code_2 = cell.ToString(); // cabinet code 2.
                                        if (queue_Node.Code_2 == null)
                                        {
                                            Console.WriteLine("Code 2 is not existing.");
                                            continue;
                                        }
                                        while (queue_Node.Code_2[0] == ' ')
                                        {
                                            queue_Node.Code_2 = queue_Node.Code_2.Substring(1);
                                        }
                                    }
                                }
                                queue_Node.Cabinet_Status = "Accepted"; // status of the cabinet.
                            }
                        }
                    }
                    info.Enqueue(queue_Node); // add the node to the queue.
                    queue_Node = new Queue_Node(); // create a new node.
                    rowIndex++; // Go fot the next row.
                    row = sheet.GetRow(rowIndex); // Get the next row.
                    Sheet_status = true; // set the sheet status to true.
                }
            }
            else
                Sheet_status = false;
        }
        return Sheet_status;
    }

    public static bool GetIPs(TextEditor T, string? Cab_Code)
    {
        // Set the cabinet status to false.
        bool Cabinet_Status = false;
        // Open the Excel file
        for (int i = 0; i < PlanIPs_filesPath.Length; i++)
        {
            using (FileStream file = new FileStream(PlanIPs_filesPath[i], FileMode.Open, FileAccess.Read))
            {
                IWorkbook? workbook = null;
                workbook = new XSSFWorkbook(file); // For .xlsx files
                                                   // Access the first worksheet
                if (workbook != null)
                {
                    ISheet sheet = workbook.GetSheetAt(0);
                    // Loop through rows and cells
                    for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        IRow row = sheet.GetRow(rowIndex);
                        if (row != null)
                        {
                            ICell cell = row.GetCell(0);
                            if (cell.ToString() == Cab_Code)
                            {
                                Cabinet_Status = true;
                                if (i == 0)
                                {
                                    // Sig Gateway IP.
                                    T.Sig_Gateway_IP = row.GetCell(2).ToString();
                                    // Sig IP Shelf 1.
                                    T.Sig_SH1_IP = row.GetCell(3).ToString();
                                    // Sig IP Shelf 2.
                                    T.Sig_SH2_IP = row.GetCell(4).ToString();

                                    // MG Gateway IP.
                                    T.Mg_Gateway_IP = row.GetCell(8).ToString();
                                    // MG IP Shelf 1.
                                    T.Mg_SH1_IP = row.GetCell(9).ToString();
                                    // MG IP Shelf 2.
                                    T.Mg_SH2_IP = row.GetCell(10).ToString();
                                    // MG IP Shelf 3.
                                    T.Mg_SH3_IP = row.GetCell(11).ToString();

                                    // FVMO EM Gateway IP.
                                    T.FVNO_EM_Gateway_IP = row.GetCell(14).ToString();
                                    // MG IP Shelf 1.
                                    T.FVNO_EM_SH1_IP = row.GetCell(15).ToString();
                                    // MG IP Shelf 2.
                                    T.FVNO_EM_SH2_IP = row.GetCell(16).ToString();
                                }
                                else
                                {
                                    // POP Name.
                                    T.POP_Name = row.GetCell(2).ToString();

                                    // TED Mg Gateway IP
                                    T.TED_Mg_Gateway_IP = row.GetCell(9).ToString();
                                    // TED Mg IP Shelf1
                                    T.TED_Mg_SH1_IP = row.GetCell(10).ToString();
                                    // TED Mg IP Shelf2 
                                    T.TED_Mg_SH2_IP = row.GetCell(11).ToString();
                                }
                                break;
                            }
                            else
                                Cabinet_Status = false;
                        }
                    }
                }
            }
            if (Cabinet_Status == false)
                break;
        }
        return Cabinet_Status;
    }
}