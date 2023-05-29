import json


def my_function(path):
    data = []
    out = []
    with open(path, 'rt') as f:
        indata = f.read()
        indata = indata.split('\n')

    for i in indata:
        data.append(i.strip())

    for i in data:
        print(i)
        out.append(json.loads(i))

    return out