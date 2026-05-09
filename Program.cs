using NTALab_3;
using System.Diagnostics;

long GetPrimeByIndex(int i)
{
    return Convert.ToInt64(File.ReadLines("primebase.txt").Skip(i).First());
}
int n;
int alpha;
int beta;
Random r = new Random();

Console.Write("p");
int p = Convert.ToInt32(Console.ReadLine());
Console.Write("alpha");
alpha = Convert.ToInt32(Console.ReadLine());
Console.Write("beta");
beta = Convert.ToInt32(Console.ReadLine());
n = p - 1;
Stopwatch sw = Stopwatch.StartNew();
int[] B = GetPB(n);
equation[] eqs;
int[] logs;
while (true)
{
    eqs = GetEqs(p, alpha, n, B);
    logs = Solve(eqs, n, B.Length);
    if (logs != null)
    {
        break;
    }
}
int x = GetX(p, alpha, beta, n, B, logs);
sw.Stop();
Console.WriteLine("x = " + x);
Console.WriteLine(sw.ElapsedMilliseconds);

int[] GetPB(int n)
{
    double B = 3.38 * Math.Exp(0.5 * Math.Sqrt(Math.Log(n) * Math.Log(Math.Log(n))));
    List<int> S = new List<int>();
    int temp;
    int i = 0;
    while (true)
    {
        temp = (int)GetPrimeByIndex(i);
        if (temp > B)
        {
            break;
        }
        i++;
        S.Add(temp);
    }
    return S.ToArray();
}

int[] GetV(int n, int[] B)
{
    int[] V = new int[B.Length];
ret:
    for (int i = 0; i < B.Length; i++)
    {
        if (n % B[i] == 0)
        {
            n = n / B[i];
            V[i]++;
            goto ret;
        }
    }
    if (n == 1)
    {
        return V;
    }
    else
    {
        return null;
    }
}

equation GetEq(int k, int p, int alpha, int n, int[] B)
{
    int[] v = GetV(ModPow(alpha, k, p), B);
    if (v == null)
    {
        return null;
    }
    return new equation(v, k % n);
}

equation[] GetEqs(int p, int alpha, int n, int[] B)
{
    List<equation> eqs = new List<equation>();
    equation eq;
    while (true)
    {
        eq = GetEq(r.Next(0, n), p, alpha, n, B);
        if (eq != null)
        {
            if (!eqs.Contains(eq))
            {
                eqs.Add(eq);
            }
            if (eqs.Count >= B.Length + 10)
            {
                return eqs.ToArray();
            }
        }
    }
}

int GetX(int p, int alpha, int beta, int n, int[] B, int[] logs)
{
    int value;
    int[] d;
    long sum;
    int l, x;
    while (true)
    {
        l = r.Next(0, n);
        value = MulMod(beta, ModPow(alpha, l, p), p);
        d = GetV(value, B);
        if (d != null)
        {
            sum = 0;
            for (int i = 0; i < B.Length; i++)
            {
                sum = sum + d[i] * logs[i];
            }
            x = Mod(sum - l, n);
            if (ModPow(alpha, x, p) == beta)
            {
                return x;
            }
        }
    }
}

int[] Solve(equation[] eqs, int n, int m) //функцію згенеровано ШІ
{
    int[,] a = new int[eqs.Length, m + 1];
    int[] pos = new int[m];

    for (int i = 0; i < m; i++)
    {
        pos[i] = -1;
    }

    for (int i = 0; i < eqs.Length; i++)
    {
        for (int j = 0; j < m; j++)
        {
            a[i, j] = Mod(eqs[i].C[j], n);
        }

        a[i, m] = Mod(eqs[i].l, n);
    }

    int row = 0;

    for (int col = 0; col < m; col++)
    {
        int sel = -1;

        for (int i = row; i < eqs.Length; i++)
        {
            if (a[i, col] != 0 && gcd(a[i, col], n) == 1)
            {
                sel = i;
                break;
            }
        }

        if (sel == -1)
        {
            continue;
        }

        SwapRows(a, row, sel, m + 1);

        int obr = inv(a[row, col], n);
        obr = Mod(obr, n);

        for (int j = col; j <= m; j++)
        {
            a[row, j] = MulMod(a[row, j], obr, n);
        }

        for (int i = 0; i < eqs.Length; i++)
        {
            if (i != row && a[i, col] != 0)
            {
                int c = a[i, col];

                for (int j = col; j <= m; j++)
                {
                    a[i, j] = Mod((long)a[i, j] - (long)c * a[row, j], n);
                }
            }
        }

        pos[col] = row;
        row++;

        if (row == m)
        {
            break;
        }
    }

    for (int i = 0; i < eqs.Length; i++)
    {
        bool zero = true;

        for (int j = 0; j < m; j++)
        {
            if (a[i, j] != 0)
            {
                zero = false;
                break;
            }
        }

        if (zero && a[i, m] != 0)
        {
            return null;
        }
    }

    for (int i = 0; i < m; i++)
    {
        if (pos[i] == -1)
        {
            return null;
        }
    }

    int[] x = new int[m];

    for (int i = 0; i < m; i++)
    {
        x[i] = a[pos[i], m];
    }

    return x;
}

void SwapRows(int[,] a, int x, int y, int m) //функцію згенеровано ШІ
{
    int temp;

    for (int i = 0; i < m; i++)
    {
        temp = a[x, i];
        a[x, i] = a[y, i];
        a[y, i] = temp;
    }

}

int ModPow(int a, int r, int m)
{
    int res = 1;
    a %= m;
    for (int i = 31; i >= 0; i--)
    {
        res = MulMod(res, res, m);
        if ((r & (1 << i)) != 0)
        {
            res = MulMod(res, a, m);
        }
    }
    return res;
}

int MulMod(int a, int b, int m)
{
    int res = 0;
    a %= m;
    while (b > 0)
    {
        if ((b & 1) != 0)
        {
            res = AddMod(res, a, m);
        }
        a = AddMod(a, a, m);
        b >>= 1;
    }
    return res;
}

int AddMod(int a, int b, int m)
{
    if (a >= m - b)
    {
        return a - (m - b);
    }
    return a + b;
}

int Mod(long a, int n)
{
    a = a % n;
    if (a < 0)
    {
        a = a + n;
    }
    return (int)a;
}

int inv(long a, long b)
{
    long t1, t2, t3, t4;
    (t1, t2, t3, t4) = Euclid(a, b, true);
    return (int)Math.Abs(t4);
}

int gcd(long a, long b)
{
    long t1, t2, t3, t4;
    (t1, t2, t3, t4) = Euclid(a, b);
    return (int)Math.Abs(t1);
}

(long, long, long, long) Euclid(long A, long B, bool uv = false)
{
    long gcd;
    long u = 0;
    long v = 0;
    long lcm;
    if (uv)
    {
        long[] t = new long[10];
        t[0] = A;
        t[1] = B;
        t[3] = t[0] / t[1];
        t[2] = t[0] % t[1];
        t[4] = 1;
        t[5] = 0;
        t[6] = 1;
        t[7] = 0;
        t[8] = 1;
        t[9] = -t[3];
        while (t[2] != 0)
        {
            t[0] = t[1];
            t[1] = t[2];
            t[3] = t[0] / t[1];
            t[2] = t[0] % t[1];
            t[4] = t[5];
            t[5] = t[6];
            t[6] = t[4] - (t[3] * t[5]);
            t[7] = t[8];
            t[8] = t[9];
            t[9] = t[7] - (t[3] * t[8]);
        }
        gcd = t[1];
        u = t[5];
        v = t[8];
    }
    else
    {
        long[] t = new long[4];
        t[0] = A;
        t[1] = B;
        t[3] = t[0] / t[1];
        t[2] = t[0] % t[1];
        while (t[2] != 0)
        {
            t[0] = t[1];
            t[1] = t[2];
            t[3] = t[0] / t[1];
            t[2] = t[0] % t[1];
        }
        gcd = t[1];
    }
    lcm = A * B / gcd;
    return (gcd, lcm, u, v);
}