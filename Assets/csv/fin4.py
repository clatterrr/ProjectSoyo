import csv

# 读取文本文件并解析
def read_text_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        lines = file.readlines()
    return lines

# 读取CSV文件并解析
def read_csv_file(file_path):
    with open(file_path, mode='r', encoding='utf-8') as csvfile:
        reader = csv.reader(csvfile)
        data = list(reader)
    return data

# 写入CSV文件
def write_csv_file(file_path, data):
    with open(file_path, mode='w', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerows(data)

# 将文本文件内容添加到CSV文件
def add_to_csv(text_lines, csv_data):
    thesummary = ""
    for line in text_lines:
        summary = line.strip()
        if len(summary.split()) == 1:  # 如果是总结
            thesummary = summary
            print(summary)
            if thesummary not in [row[0] for row in csv_data]:  # 如果总结不在CSV中
                csv_data.append([thesummary, ''])  # 在文件最后添加新的总结行
                print("add new")
            else:  # 如果总结已存在，找到对应的行
                summary_index = [i for i, row in enumerate(csv_data) if row[0] == thesummary]
                for i in summary_index:
                    # 在已有的总结下插入一行，第一列为总结，第二列为空
                    csv_data.insert(i + 1, [thesummary, ''])  # 插入新的行
                    break
                print("insert new under existing")
        else:  # 如果是短句
            for i, row in enumerate(csv_data):
                if row[0] == thesummary and not row[1]:  # 检查第一列是否匹配且第二列为空
                    csv_data[i] = [row[0], summary.strip() + '\n']  # 填充第二列
                    break
    return csv_data

text_file_path = 'test.txt'  # 文本文件路径
csv_file_path = 'StoryL2.xlsx'  # CSV文件路径

# 读取文本文件和CSV文件
text_lines = read_text_file(text_file_path)
csv_data = read_csv_file(csv_file_path)

# 将文本文件内容添加到CSV文件
csv_data = add_to_csv(text_lines, csv_data)

# 写入CSV文件
write_csv_file(csv_file_path, csv_data)

def clean_text_file(file_path):
    cleaned_lines = []

    # 读取并过滤每一行
    with open(file_path, 'r', encoding='utf-8') as file:
        for line in file:
            stripped_line = line.strip()
            # 保留不为空且只有一个单词的行
            if stripped_line and len(stripped_line.split()) == 1:
                cleaned_lines.append(stripped_line)

    # 覆盖写入原文件
    with open(file_path, 'w', encoding='utf-8') as file:
        for line in cleaned_lines:
            file.write(line + '\n')

clean_text_file(text_file_path)