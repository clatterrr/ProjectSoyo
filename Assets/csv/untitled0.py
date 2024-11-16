import openpyxl
import re

# 读取xlsx文件
def read_xlsx(file_path):
    wb = openpyxl.load_workbook(file_path)
    sheet = wb.active
    summary_words = []
    common_words = []
    for row in sheet.iter_rows(min_row=2, values_only=True):  # 假设第一行是标题行
        summary_word = row[0]
        common_words.extend(row[1:])
        summary_words.append(summary_word)
    return summary_words, common_words

# 读取txt文件并处理
def process_txt(file_path, summary_words, common_words):
    with open(file_path, 'r', encoding='utf-8') as file:
        lines = file.readlines()
        
    for line in lines:
        line = line.split('\n')[0]
        if line.strip():
            pre_words = []
            for i in range(len(common_words)):
                if common_words[i] != None and common_words[i] in line:
                    theword = common_words[i]
                    index = int(i / 5)
                    thatword = summary_words[index]
                    pre_words.append(summary_words[index])
            print(pre_words)
            print(line)
    

xlsx_file_path = 'Tia.xlsx'  # xlsx文件路径
txt_file_path = 'w2.txt'  # txt文件路径

summary_words, common_words = read_xlsx(xlsx_file_path)
process_txt(txt_file_path, summary_words, common_words)
    
