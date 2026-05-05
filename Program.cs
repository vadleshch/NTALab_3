using NTALab_3;

long GetPrimeByIndex(int i)
{
    return Convert.ToInt64(File.ReadLines("primebase.txt").Skip(i).First());
}
int n = 2048;
int alpha;
int beta;
Random r = new Random();

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
