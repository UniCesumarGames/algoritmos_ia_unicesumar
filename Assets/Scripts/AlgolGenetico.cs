using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AlgolGenetico : MonoBehaviour
{
    public GameObject prefabEsfera;
    public Transform alvo;

    public int tamanhoPopulacao = 2;
    public int quantidadePassos = 60;
    public float forcaMovimento = 10f;
    public float tempoEntrePassos = 0.05f;
    public float taxaMutacao = 0.01f;
    public float bonusSucesso = 20f;

    private List<Vector2[]> populacao = new List<Vector2[]>();
    private List<float> listFitness = new List<float>();
    private int geracaoAtual = 0;
       

    void Start()
    {
        Debug.Log("Algoritmo iniciado!");
        for(int i = 0; i <tamanhoPopulacao; i++)
        {
            populacao.Add(GerarDNA());
        }
        StartCoroutine(CicleGenetico());
    }

    Vector2[] GerarDNA()
    {
        Vector2[] dna = new Vector2[quantidadePassos];
        for (int i = 0; i < quantidadePassos; i++)
        {
            dna[i] = Random.insideUnitCircle;
        }
        return dna;
    }

    IEnumerator CicleGenetico()
    {
        while (true)
        {
            geracaoAtual++;
            listFitness.Clear();

            Debug.Log($"=== GERAÇÃO === {geracaoAtual}");

            for (int i = 0; i < populacao.Count; i++)
            {
                yield return TestarIndividuo(populacao[i], false);
            }

            int indiceMelhor = 0;

            for (int i = 1; i <listFitness.Count; i++)
            {
                if (listFitness[i] > listFitness[indiceMelhor])
                {
                    indiceMelhor = i;
                }
            }

            Debug.Log($"Melhor Fitness da geração: {listFitness[indiceMelhor]}:F2");

            yield return TestarIndividuo(populacao[indiceMelhor], true);

            List<Vector2[]> novaPopulacao = new List <Vector2[]>();

            novaPopulacao.Add((Vector2[])populacao[indiceMelhor].Clone());

            while(novaPopulacao.Count < tamanhoPopulacao)
            {
                Vector2[] filho = (Vector2[])populacao[indiceMelhor].Clone();
                for (int g = 0; g < quantidadePassos; g++)
                {
                    if(Random.value < taxaMutacao)
                    {
                        filho[g] = Random.insideUnitCircle;
                    }
                }
                novaPopulacao.Add(filho);
            }
            populacao = novaPopulacao;
        }
    }


    IEnumerator TestarIndividuo(Vector2[] dna, bool modoDemo)
    {
        GameObject esfera = Instantiate(prefabEsfera);
        Rigidbody rb = esfera.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        esfera.transform.position = new Vector3 (0, 1, 0);

        bool chegouNoAlvo = false;

        for (int i = 0; i <quantidadePassos; i++)
        {
            Vector2 gene = dna[i];
            rb.AddForce(new Vector3(gene.x, 0, gene.y) * forcaMovimento, ForceMode.Acceleration);

            if (modoDemo)
                Debug.DrawLine(esfera.transform.position, alvo.position, Color.yellow, tempoEntrePassos);

            float distance = Vector3.Distance(esfera.transform.position, alvo.position);

            if (distance < 1f)
            {
                chegouNoAlvo = true;
                break;
            }

            yield return new WaitForSeconds(tempoEntrePassos);
        }

        float distanciaFinal = Vector3.Distance(esfera.transform.position, alvo.position);

        float fitness = -distanciaFinal;

        if (chegouNoAlvo)
            fitness += bonusSucesso;

        listFitness.Add(fitness);

        if (modoDemo)
        {
            Debug.Log($"Demo | Distância Final: {distanciaFinal:F2} " +
                $"| Fitness: {fitness:F2} | Chegou? {(chegouNoAlvo ? "SIM" : "NÃO")}");
        }
        Destroy(esfera);
    }

}
