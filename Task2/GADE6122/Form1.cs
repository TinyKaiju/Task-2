using System; // Yunus Joosub & Thoriso Tlale
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GADE6122
{
    public partial class Form1 : Form
    {
        //Question 2.1
        public abstract class Tile
        {
            protected int x;
            protected int y;

            public enum tiletype { Hero, Enemy, Gold, Weapon };
            public tiletype type;

            public Tile(int a, int b) //Constructor
            {
                this.x = a;
                this.y = b;
            }

            public int getX()
            {
                return this.x;
            }
            public int getY()
            {
                return this.y;
            }

            public tiletype getTiletype()
            {
                return type;
            }
        }

        public class Obstacle : Tile
        {
            public Obstacle(int x, int y) : base(x, y)
            { }
        }

        public class EmptyTile : Tile
        {
            public EmptyTile(int x, int y) : base(x, y)
            { }
        }
        //Question 2.2
        public abstract class Character : Tile
        {
            protected int hp;
            protected int maxHp;
            protected int damage;
            protected bool cornerVision = false;
            protected Tile[] visionTiles; //In ArrayVision = North, East, South, West
            public enum movementEnum { None, Up, Down, Left, Right };

            private char symbol;

            protected int goldpurse;
            //Get methods
            public int getHp()
            {
                return this.hp;
            }
            public int getMaxHp()
            {
                return this.maxHp;
            }
            public int getDamage()
            {
                return this.damage;
            }

            public char getSymbol()
            {
                return symbol;
            }

            public Character(int a, int b, char symbol) : base(a, b) // constructor
            {
                this.symbol = symbol;
            }

            public virtual void Attack(Character target)
            { }
            public bool isDead()
            {
                if (this.hp <= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public virtual bool CheckRange(Character target)
            {
                if ((DistanceTo(target)) == 1)
                {
                    return true;
                }
                return false;
            }
            private int DistanceTo(Character target) // Incomplete
            {
                int x = target.getX();
                int y = target.getY();

                int distance = (Math.Abs(this.getX() - x)) + Math.Abs((this.getY() - y));
                if (this is Mage)// Check if works properly
                {
                    if ((Math.Abs(this.getX() - x) == 2) && Math.Abs((this.getY() - y)) == 1)
                    {
                        distance = 1;
                    }
                    else if ((Math.Abs(this.getX() - x) == 1) && Math.Abs((this.getY() - y)) == 2)
                    {
                        distance = 1;
                    }
                }

                return distance;
            }
            public void Move(movementEnum move)
            {
                switch (move)
                {
                    case movementEnum.Up:
                        this.x--;
                        break;
                    case movementEnum.Down:
                        this.x++;
                        break;
                    case movementEnum.Right:
                        this.y++;
                        break;
                    case movementEnum.Left:
                        this.y--;
                        break;
                }

            }
            public abstract movementEnum ReturnMove(movementEnum move);
            //public abstract override ToString() { }

            //vision Setting
            public int getVisionSize()
            {
                return visionTiles.Length;
            }
            public void setVisionTiles(Tile[,] map)
            {
                this.visionTiles[0] = map[x - 1, y];
                this.visionTiles[1] = map[x + 1, y];
                this.visionTiles[2] = map[x, y + 1];
                this.visionTiles[3] = map[x, y - 1];
                if (cornerVision)
                {
                    this.visionTiles[4] = map[x - 1, y - 1];
                    this.visionTiles[5] = map[x + 1, y - 1];
                    this.visionTiles[6] = map[x - 1, y + 1];
                    this.visionTiles[7] = map[x + 1, y + 1];
                }


            }
            public void Pickup(Item i)
            {
                
                if (i is Gold)
                {
                    Gold goldnum;
                    goldnum = (Gold)i;
                    goldpurse += goldnum.getGoldAmount();
                }

                
            }
            public Tile getVisionTile(int i)
            {
                return visionTiles[i];
            }
            public void damaged(int dmg)
            {
                this.hp -= dmg;
            }
        }

        //Question 2.4
        public abstract class Enemy : Character
        {
            protected Random randNum = new Random();
            protected bool friendlyFire = false;
            public Enemy(int x, int y, int Damage, int Maxhp, char Symbol) : base(x, y, Symbol) //Constructor
            {
                this.damage = Damage;
                this.maxHp = Maxhp;
                this.hp = Maxhp;
                this.type = tiletype.Enemy;
            }

            public bool getFriendyFire()
            {
                return friendlyFire;
            }
            public override string ToString()
            {
                return "Goblin at [" + x + "," + y + "] (" + damage + ")"; // double check 
            }
        }

        //Question 2.5
        public class Goblin : Enemy
        {
            public Goblin(int x, int y) : base(x, y, 1, 10, 'G')//Constructor
            {
                this.visionTiles = new Tile[4];
            }
            public override movementEnum ReturnMove(movementEnum move)
            {
                int possible = 4;
                for (int i = 0; i < 4; i++)
                {
                    if (visionTiles[i] is not EmptyTile || (visionTiles[i] is not Gold))
                    {
                        possible--;
                    }
                }

                if (possible == 0)
                {
                    return movementEnum.None;
                }
                move = movementEnum.Up;
                int direct = randNum.Next(4);

                Tile temp = new Obstacle(0, 0);
                while ((temp is not EmptyTile) || temp is not Gold)
                {
                    switch (direct)
                    {
                        case 0:
                            temp = visionTiles[0];
                            move = movementEnum.Up;
                            break;
                        case 1:
                            temp = visionTiles[1];
                            move = movementEnum.Down;
                            break;
                        case 2:
                            temp = visionTiles[2];
                            move = movementEnum.Right;
                            break;
                        case 3:
                            temp = visionTiles[3];
                            move = movementEnum.Left;
                            break;
                        default:
                            direct = randNum.Next(4);
                            break;
                    }
                    direct = randNum.Next(4);

                }
                return move;
            }
        }

        public class Mage : Enemy // Task2 Question 2.3
        {
            public Mage(int x, int y) : base(x, y, 5, 5, 'M')
            {
                this.visionTiles = new Tile[8];
                friendlyFire = true;
                this.cornerVision = true;
            }

            public override movementEnum ReturnMove(movementEnum move)
            {
                return 0;
            }

            public override bool CheckRange(Character target)
            {
                return base.CheckRange(target);
            }

            public override string ToString()
            {
                return "Mage at [" + x + "," + y + "] (" + damage + ")"; // double check 
            }

        }
        public class Hero : Character
        {
            public Hero(int x, int y, int hp) : base(x, y, 'H')//Constructor
            {
                this.visionTiles = new Tile[4];
                this.damage = 2;
                this.maxHp = hp;
                this.hp = hp;
                this.type = tiletype.Hero;
            }
            public override movementEnum ReturnMove(movementEnum move)
            {
                return move;
            }
            public override string ToString()
            {
                return ("Player Stats:\n HP: " + hp + "/" + maxHp + "\n Damage: " + damage + "\n Gold: " + goldpurse + "\n [" + getX() + "," + getY() + "]");
            }

            public override bool CheckRange(Character target)
            {
                return base.CheckRange(target);
            }

            public override void Attack(Character target)
            {
                target.damaged(this.damage);
            }
        }

        //Question 2.2.1
        public abstract class Item : Tile
        {
            public Item(int x, int y) : base(x, y)
            {
                this.type = tiletype.Gold;

            }
            public abstract new string ToString();
            protected string ItemType;

        }

        //Qustion 2.2.2
        public class Gold : Item
        {
            private int goldAmount;
            private Random randomGoldAmount = new Random();

            public Gold(int x, int y) : base(x, y) //Constructor
            {
                goldAmount = randomGoldAmount.Next(1, 5);
                this.ItemType = "Gold";
            }

            public int getGoldAmount()
            {
                return this.goldAmount;
            }
            public override string ToString()
            {
                return this.ItemType;
            }

        }

        public class Map
        {

            private Tile[,] mapTiles;
            private Hero player;
            private Enemy[] enemies;
            private int mapWidth;
            private int mapHeight;
            private Random randomNum = new Random();
            //private int gold;
            //private Gold[] goldArray;
            private Item[] Items;
            public Map(int wMin, int wMax, int hMin, int hMax, int enemyNum, int goldNum)
            {


                mapWidth = randomNum.Next(wMin, wMax) + 2;
                mapHeight = randomNum.Next(hMin, hMax) + 2;

                enemies = new Enemy[enemyNum];
                mapTiles = new Tile[mapWidth, mapHeight];

                fillMap();

                player = (Hero)Create(Tile.tiletype.Hero); //cast
                mapTiles[player.getX(), player.getY()] = player;

                for (int i = 0; i < enemyNum; i++)
                {
                    enemies[i] = (Enemy)Create(Tile.tiletype.Enemy);
                    mapTiles[enemies[i].getX(), enemies[i].getY()] = enemies[i];
                }


                Items = new Gold[goldNum];
                for (int i = 0; i < goldNum; i++)
                {
                    Items[i] = (Gold)Create(Tile.tiletype.Gold);
                    mapTiles[Items[i].getX(), Items[i].getY()] = Items[i];
                }


                UpdateVision();

            }

            public void UpdateVision() // used to set vision after moving
            {
                player.setVisionTiles(mapTiles);
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].setVisionTiles(mapTiles);
                }
            }

            private Tile Create(Tile.tiletype type)
            {
                int uniqueX = randomNum.Next(mapWidth - 1);
                int uniqueY = randomNum.Next(mapHeight - 1);
                while ((mapTiles[uniqueX, uniqueY] is not EmptyTile) && (mapTiles[uniqueX, uniqueY] is not EmptyTile))
                {
                    uniqueX = randomNum.Next(mapWidth - 1);
                    uniqueY = randomNum.Next(mapHeight - 1);
                }


                switch (type)  // create tile 
                {
                    case Tile.tiletype.Hero:
                        return new Hero(uniqueX, uniqueY, 50);
                    case Tile.tiletype.Enemy:
                        int rand = randomNum.Next(2);
                        switch (rand) // Randomise enemy type
                        {
                            case 0: return new Goblin(uniqueX, uniqueY);
                            case 1: return new Mage(uniqueX, uniqueY);
                            default: return new EmptyTile(uniqueX, uniqueY);
                        }
                    case Tile.tiletype.Gold:
                        return new Gold(uniqueX, uniqueY);
                    default: return new EmptyTile(uniqueX, uniqueY);
                }
            }

            public void fillMap()
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        mapTiles[x, y] = new EmptyTile(x, y);
                    }
                }
                for (int x = 0; x < mapWidth; x++)
                {
                    mapTiles[x, 0] = new Obstacle(x, 0);
                    mapTiles[x, mapHeight - 1] = new Obstacle(x, mapHeight - 1);
                }
                for (int y = 0; y < mapHeight; y++)
                {
                    mapTiles[0, y] = new Obstacle(0, y); //top row
                    mapTiles[mapWidth - 1, y] = new Obstacle(mapWidth - 1, y);//bottom row
                }
            }

            public int getWidth()
            {
                return mapWidth;
            }
            public int getHeight()
            {
                return mapHeight;
            }
            public Tile getMapTiles(int x, int y)
            {
                return mapTiles[x, y];
            }

            public int getPlayerX()
            {
                return player.getX();
            }
            public int getPlayerY()
            {
                return player.getY();
            }

            public void Move(Character.movementEnum move)
            {
                int oldX = getPlayerX();
                int oldY = getPlayerY();
                player.Move(move);

                mapTiles[oldX, oldY] = new EmptyTile(oldX, oldY);
                mapTiles[getPlayerX(), getPlayerY()] = player;

                UpdateVision();
            }
            public void Move(Character.movementEnum move, Item pick)
            {
                player.Pickup(pick);
                int oldX = getPlayerX();
                int oldY = getPlayerY();
                player.Move(move);

                mapTiles[oldX, oldY] = new EmptyTile(oldX, oldY);
                mapTiles[getPlayerX(), getPlayerY()] = player;

                UpdateVision();
            }

            public List<Enemy> getTargetEnemies() // Creates list of enemies in range of attack
            {
                int n = 0;
                List<Enemy> enemyTargets = new List<Enemy>();

                for (int t = 0; t < enemies.Length; t++)
                {

                    if (player.CheckRange(enemies[t]))
                    {
                        enemyTargets.Add(enemies[t]);
                        n++;
                    }
                }
                return enemyTargets;
            }

            public string tryAttack(Enemy target) // Check to see if attack is possible and successful
            {
                List<Enemy> enemies = getTargetEnemies();
                if (enemies.Contains(target))
                {
                    player.Attack(target);

                    if (!target.isDead())
                    {
                        return target.ToString() + "HP: " + Convert.ToString(target.getHp());
                    }
                    else
                    {
                        int x = target.getX();
                        int y = target.getY();
                        String output = "Killed " + target.ToString();

                        mapTiles[x, y] = new EmptyTile(x, y);
                        removeEnemy(target);

                        return output;
                    }
                }
                else
                {
                    return "Attack failed";
                }
            }

            public void tryEnemyAttack()
            {
                Enemy attacker;
                for (int e = 0; e < enemies.Length; e++)
                {
                    attacker = enemies[e];
                    for (int i = 0; i < attacker.getVisionSize(); i++)
                    {
                        if ((attacker.getFriendyFire()) && (attacker.getVisionTile(i) is Enemy))
                        {
                            Tile temp = attacker.getVisionTile(i);
                            Enemy victim = (Enemy)mapTiles[temp.getX(), temp.getY()];
                            
                        }
                        else if (attacker.getVisionTile(i) is Hero)
                        {
                            player.damaged(attacker.getDamage());
                        }
                    }

                }
            }
            public void removeEnemy(Enemy target) // Removes enemy from array and resizes it
            {
                int j = 0;
                Enemy[] temp = new Enemy[enemies.Length - 1];

                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] != target)
                    {
                        temp[j] = enemies[i];
                        j++;
                    }
                }
                enemies = new Enemy[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    enemies[i] = temp[i];
                }
            }

            public virtual string getPlayerInfo()
            {
                return player.ToString();
            }

            public virtual string getEnemyInfo(Enemy target)
            {
                return target.ToString() + "\nHP: " + Convert.ToString(target.getHp());
            }

            public int getEnemyarraySize()
            {
                return enemies.Length;
            }
            public Enemy getEnemy(int i)
            {
                return enemies[i];
            }

            public void moveEnemies()
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    int oldX = enemies[i].getX();
                    int oldY = enemies[i].getY();
                    enemies[i].Move(enemies[i].ReturnMove(0));

                    mapTiles[oldX, oldY] = new EmptyTile(oldX, oldY);
                    mapTiles[enemies[i].getX(), enemies[i].getY()] = enemies[i];
                    UpdateVision();
                }
            }

            public Item getItemAtPosition(int x, int y)
            {
                if (mapTiles[x, y] is Item)
                {
                    return (Item)mapTiles[x, y];
                }
                else
                {
                    return null;
                }
            }

        }

        //Question 3.3
        public class GameEngine
        {
            private const char emptyChar = ' ';
            private const char obstacleChar = 'X';
            private const char goldchar = '$';
            private Map map;

            public GameEngine(int widthMin, int widthMax, int heightMin, int heightMax, int enemyNum, int goldNum) // constructor
            {
                map = new Map(widthMin, widthMax, heightMin, heightMax, enemyNum, goldNum);
            }

            public bool MovePlayer(Character.movementEnum moveType)
            {
                int x, y;
                x = map.getPlayerX();
                y = map.getPlayerY();

                switch (moveType)
                {
                    case Character.movementEnum.Up:
                        x--;
                        break;
                    case Character.movementEnum.Down:
                        x++;
                        break;
                    case Character.movementEnum.Right:
                        y++;
                        break;
                    case Character.movementEnum.Left:
                        y--;
                        break;
                }

                if (map.getMapTiles(x, y) is EmptyTile || map.getMapTiles(x, y) is Item) // player can move
                {
                    Item tempItem = map.getItemAtPosition(x, y);

                    if (tempItem != null)
                    {
                        map.Move(moveType, tempItem);
                    }
                    else
                    {
                        map.Move(moveType);
                    }
                    
                    moveEnemy();
                    map.tryEnemyAttack();
                    return true;
                }
                else
                {
                    return false; //nothing
                }

            }

            public Map getMap()
            {
                return this.map;
            }
            public string getPlayerInfo()
            {
                return map.getPlayerInfo();
            }
            public string getEnemyInfo(Enemy target)
            {
                return map.getEnemyInfo(target);
            }

            public void moveEnemy()
            {
                map.moveEnemies();
            }
            public override string ToString()
            {

                string output = "";
                for (int x = 0; x < map.getWidth(); x++)
                {
                    for (int y = 0; y < map.getHeight(); y++)
                    {
                        if (map.getMapTiles(x, y) is Gold)
                        {
                            output += '$';
                        }
                        if (map.getMapTiles(x, y) is Character)
                        {
                            Character temp = (Character)map.getMapTiles(x, y);
                            output += temp.getSymbol();
                        }
                        else
                        {
                            switch (map.getMapTiles(x, y))
                            {
                                case Obstacle:
                                    output += obstacleChar;
                                    break;

                                case EmptyTile:
                                    output += emptyChar;
                                    break;
                            }
                        }
                    }
                    output += "\n";
                }
                return output;
            }

            public List<Enemy> getTargets()
            {
                return map.getTargetEnemies();
            }

            public string tryAttack(Enemy target)
            {
                return map.tryAttack(target);
                //map.tryEnemyAttack();
            }

            public void EnemiesAtk()
            {
                map.tryEnemyAttack();
            }

            
        }
        GameEngine game;
        public Form1()
        {
            InitializeComponent();
            game = new GameEngine(10, 20, 10, 20, 5, 3);
            updateForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            game.MovePlayer(Character.movementEnum.Up);
            updateForm();

        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            game.MovePlayer(Character.movementEnum.Down);
            updateForm();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            game.MovePlayer(Character.movementEnum.Left);
            updateForm();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            game.MovePlayer(Character.movementEnum.Right);
            updateForm();
        }

        public void updateAttackList()
        {
            CmbEnemyList.Items.Clear();
            CmbEnemyList.Text = "";
            for (int i = 0; i < game.getTargets().Count(); i++)
            {
                CmbEnemyList.Items.Add(game.getTargets().ElementAt(i));
            }
            if (CmbEnemyList.Items.Count != 0)
            {
                CmbEnemyList.SelectedIndex = 0;
                updateEnemy();
            }
        }

        public void updateForm()
        {
            rtbMap.Clear();
            rtbMap.Text = game.ToString();
            MemoPlayerInfo.Text = game.getPlayerInfo();
            updateAttackList();
            updateEnemy();
            lblOutput.Text = "";
        }

        private void BtnAttack_Click(object sender, EventArgs e)
        {

            if (CmbEnemyList.Items.Count != 0)
            {
                int i = CmbEnemyList.SelectedIndex;
                Enemy target = (Enemy)CmbEnemyList.Items[i];
                lblOutput.Text = game.tryAttack(target);
                game.EnemiesAtk();
                MemoPlayerInfo.Text = game.getPlayerInfo();
                rtbMap.Clear();
                rtbMap.Text = game.ToString();
                updateAttackList();
                updateEnemy();
            }
            else
            {
                lblOutput.Text = "No Targets Available";
            }
        }

        private void CmbEnemyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateEnemy();
        }

        public void updateEnemy()
        {
            MemoEnemyInfo.Clear();
            if (CmbEnemyList.Items.Count != 0)
            {
                int i = CmbEnemyList.SelectedIndex;
                Enemy target = (Enemy)CmbEnemyList.Items[i];
                MemoEnemyInfo.Text = game.getEnemyInfo(target);
            }
        }
    }
}
