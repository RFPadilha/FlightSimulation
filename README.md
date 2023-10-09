# FlightSimulation
Simulador de vôo baseado em física, jogável com teclado ou gamepad.

Versão do Unity utilizada: 2019.4.11f1

## Scripts
3 Definições de Assembly foram criadas para separar os scripts em categorias enquanto otimiza o tempo de compilação.

-Items: Contém os scripts que regulam o comportamento dos anéis que o jogador coleta e do objeto de jogo que gera os anéis ao iniciar o jogo. A contagem da pontuação e do multiplicador foi feita através de eventos, onde cada script que deveria
reagir ao evento "assina" aquele evento, descrevendo como reagir quando o evento é invocado. No caso do anel, o evento é invocado somente ao jogador passar através da caixa de colisão.

-Player: contém a definição do InputSystem atualizado do Unity juntamente com a classe gerada automaticamente, também contém o script dos controles do jogador, que utiliza os inputs para calcular a movimentação do avião baseado em física. 
O script de controle da câmera permite que o jogador olhe ao redor do avião, podendo encontrar os anéis com mais facilidade, e finalmente, o script "MovePlaneParts" cuida das partes móveis do corpo do avião, sem influenciar na movimentação.
A definição do InputSystem foi o que permitiu a configuração do suporte a teclado e gamepad simultaneamente, sem grande esforço para implementação ou mudanças.

-UI: Cada script nessa definição é responsável pelo comportamento de um elemento da UI, bússola, angulação vertical, linha do horizonte, minimapa, HUD do jogador(velocidade, aceleração, altímetro e força G) compõem as informações que 
auxiliam o jogador a navegar. ScoreCounter, Timer, EndGameStats e UIManager regulam as regras de jogo descritas no documento fornecido e controlam as transições de UI. O Timer utiliza um evento para acionar o mecanismo de término de jogo 
quando chega a zero.

## Assets Utilizados

Probuilder: excelente ferramenta para a construção de polígonos e objetos, utilizado para construir os anéis

_HeathenEngineering, JohnFarmer e AccessibleAudioToolkit: assets gratuitos da asset store que forneceram ícones e alguns outros recursos de UI. No final nem todos foram utilizados mas todos fizeram parte dos testes na etapa de desenvolvimento.

Simple_Low_Poly_Plane: modelo do avião utilizado, fornecendo somente o objeto 3D.

## Game Objects

O único prefab criado foi o anel que o jogador coleta, contendo mesh, trigger para detectar coleta, e sistema de partícula.

UIRoot: GameObject que contém todos os scripts de UI, com exceção do "MinimapScript", para fácil acesso, concentrando os scripts em um único objeto enquanto cada um possui sua própria função.

MinimapCamera: Câmera ortográfica que renderiza o minimapa, utiliza do "MinimapScript" para permanecer centrada no jogador.

RingSpawner: Objeto que contém o script que define as regras de geração procedural dos anéis.

Player: Contém o modelo 3D do avião junto com suas partes móveis, a câmera principal e um ícone para fácil identificação no minimapa. Também é o objeto que utiliza os scripts da definição de assembly "Player".

## Disclaimer

O plano original era desenvolver esse projeto utilizando o ECS do unity e maximizar a performance com o sistema de Jobs e o Burst Compiler, porém estou lidando com problemas de performance na máquina atual de 2015, requerindo um tempo de espera 
entre 10 a 20 minutos para cada mudança de script, pelo que vi isso é um problema crescente com as versões novas do Unity que diversos desenvolvedores estão enfrentando; como o teste possui um limite de tempo, julguei que esse tempo é indispensável 
na hora de desenvolver. Esse problema será resolvido ao final do mês com a aquisição de uma nova máquina com mais RAM e processamento.
