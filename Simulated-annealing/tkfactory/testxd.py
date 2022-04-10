import matplotlib.pyplot as plt
import os
import numpy as np
from scipy import signal


def fgaussian(size, sigma):
    m = n = size
    h, k = m // 2, n // 2
    x, y = np.mgrid[-h:h + 1, -k:k + 1]
    g = np.exp(-(x ** 2 + y ** 2) / (2 * sigma ** 2))
    return g / g.sum()


sigmas = [-1, 0.1, 0.5, 0.9, 1, 5]
size = 5

f, axs = plt.subplots(len(sigmas),2, dpi=200, figsize=(14, 15))
plt.gray()
for i in range(len(sigmas)):
    M = fgaussian(size, sigmas[i])

    axs[i][0].imshow(dst)
    axs[i][1].gca(projection='3d')
    axs[i][1]
    
    # axs[i][1].grid()
plt.show()