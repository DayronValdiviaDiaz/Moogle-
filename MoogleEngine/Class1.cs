using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Matriz
{
    private int[,] elementos;
    private int filas, columnas;

    public Matriz(int[,] elementos)
    {
        this.elementos = elementos;
        filas = elementos.GetLength(0);
        columnas = elementos.GetLength(1);
    }

    public Matriz(int filas, int columnas)
    {
        this.filas = filas;
        this.columnas = columnas;
        elementos = new int[filas, columnas];
    }

    public int Filas { get { return filas; } }
    public int Columnas { get { return columnas; } }

    public int this[int fila, int columna]
    {
        get { return elementos[fila, columna]; }
        set { elementos[fila, columna] = value; }
    }

    public static Matriz operator +(Matriz a, Matriz b)
    {
        if (a.Filas != b.Filas || a.Columnas != b.Columnas)
            throw new Exception("No se pueden sumar matrices de diferentes dimensiones.");

        int[,] resultado = new int[a.Filas, a.Columnas];
        for (int i = 0; i < a.Filas; i++)
        {
            for (int j = 0; j < a.Columnas; j++)
            {
                resultado[i, j] = a[i, j] + b[i, j];
            }
        }

        return new Matriz(resultado);
    }

    public static Matriz operator -(Matriz a, Matriz b)
    {
        if (a.Filas != b.Filas || a.Columnas != b.Columnas)
            throw new Exception("No se pueden restar matrices de diferentes dimensiones.");

        int[,] resultado = new int[a.Filas, a.Columnas];
        for (int i = 0; i < a.Filas; i++)
        {
            for (int j = 0; j < a.Columnas; j++)
            {
                resultado[i, j] = a[i, j] - b[i, j];
            }
        }

        return new Matriz(resultado);
    }

    public static Matriz operator *(Matriz a, Matriz b)
    {
        if (a.Columnas != b.Filas)
            throw new Exception("Las dimensiones de las matrices no son compatibles para multiplicar.");

        int[,] resultado = new int[a.Filas, b.Columnas];
        for (int i = 0; i < a.Filas; i++)
        {
            for (int j = 0; j < b.Columnas; j++)
            {
                for (int k = 0; k < a.Columnas; k++)
                {
                    resultado[i, j] += a[i, k] * b[k, j];
                }
            }
        }

        return new Matriz(resultado);
    }

    public static Matriz operator *(Matriz a, int escalar)
    {
        int[,] resultado = new int[a.Filas, a.Columnas];
        for (int i = 0; i < a.Filas; i++)
        {
            for (int j = 0; j < a.Columnas; j++)
            {
                resultado[i, j] = a[i, j] * escalar;
            }
        }

        return new Matriz(resultado);
    }

    public static Matriz operator *(int escalar, Matriz a)
    {
        return a * escalar;
    }

    public static Matriz operator *(Matriz a, int[] vector)
    {
        if (a.Columnas != vector.Length)
            throw new Exception("Las dimensiones de la matriz y el vector no son compatibles para multiplicar.");

        int[,] resultado = new int[a.Filas, 1];
        for (int i = 0; i < a.Filas; i++)
        {
            for (int j = 0; j < a.Columnas; j++)
            {
                resultado[i, 0] += a[i, j] * vector[j];
            }
        }

        return new Matriz(resultado);
    }
}