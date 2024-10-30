import csv

# 定义CSV文件名和TXT文件名
csv_filename = 'output.csv'
txt_filename = 'rewrite.txt'

# 用于存储已经输出过的总结
summaries_seen = set()

# 读取CSV文件并写入TXT文件
with open(csv_filename, mode='r', encoding='utf-8') as csvfile, \
     open(txt_filename, mode='w', encoding='utf-8') as txtfile:
    csvreader = csv.reader(csvfile)
    for row in csvreader:
        # 假设第一列是总结，第二列是句子
        summary = row[0]
        sentence = row[1].strip()  # 去除句子两端的空白字符
        if summary not in summaries_seen:
            # 输出总结和句子
            txtfile.write('\n' + summary + '\n' + sentence + '\n')
            # 将总结添加到已见集合中
            summaries_seen.add(summary)
        else:
            txtfile.write(sentence + '\n')

print(f'TXT file {txt_filename} has been created.')