# Configuração do MariaDB para AvenSuites-Api

## 🔧 **Configuração Atual**

A API está configurada para usar **MariaDB** com as seguintes configurações:

### 📋 **String de Conexão**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AvenSuitesDb;Uid=root;Pwd=15Dev;Port=3306;"
  }
}
```

### 🗄️ **Parâmetros**
- **Servidor**: localhost
- **Porta**: 3306 (padrão MariaDB)
- **Usuário**: root
- **Senha**: 15Dev
- **Banco**: AvenSuitesDb

---

## 🚀 **Passos para Configurar MariaDB**

### **1. Verificar se MariaDB está rodando**
```bash
# Windows (como administrador)
net start mysql

# Ou verificar serviços
services.msc
```

### **2. Conectar via MySQL Workbench**
1. Abra o **MySQL Workbench**
2. Crie uma nova conexão:
   - **Hostname**: localhost
   - **Port**: 3306
   - **Username**: root
   - **Password**: 15Dev

### **3. Criar o banco de dados**
```sql
-- Conecte-se como root e execute:
CREATE DATABASE AvenSuitesDb;
CREATE DATABASE AvenSuitesDb_Dev;

-- Verificar se foi criado
SHOW DATABASES;
```

### **4. Verificar usuário root**
```sql
-- Verificar usuários
SELECT User, Host FROM mysql.user WHERE User = 'root';

-- Se necessário, criar/alterar senha do root
ALTER USER 'root'@'localhost' IDENTIFIED BY '15Dev';
FLUSH PRIVILEGES;
```

---

## 🔍 **Solução de Problemas**

### **Erro: Access denied for user 'root'@'localhost'**

#### **Solução 1: Resetar senha do root**
```sql
-- Parar MariaDB
-- Iniciar em modo seguro (sem senha)
mysqld --skip-grant-tables

-- Em outro terminal, conectar sem senha
mysql -u root

-- Alterar senha
USE mysql;
UPDATE user SET authentication_string = PASSWORD('15Dev') WHERE User = 'root';
FLUSH PRIVILEGES;
EXIT;

-- Reiniciar MariaDB normalmente
```

#### **Solução 2: Criar novo usuário**
```sql
-- Conectar como root (se possível)
CREATE USER 'avensuites'@'localhost' IDENTIFIED BY '15Dev';
GRANT ALL PRIVILEGES ON AvenSuitesDb.* TO 'avensuites'@'localhost';
GRANT ALL PRIVILEGES ON AvenSuitesDb_Dev.* TO 'avensuites'@'localhost';
FLUSH PRIVILEGES;
```

E alterar a string de conexão:
```json
"DefaultConnection": "Server=localhost;Database=AvenSuitesDb;Uid=avensuites;Pwd=15Dev;Port=3306;"
```

### **Erro: Can't connect to MySQL server**

#### **Verificar se MariaDB está rodando**
```bash
# Windows
net start mysql

# Verificar porta
netstat -an | findstr 3306
```

#### **Verificar configuração do MariaDB**
- Arquivo de configuração: `my.ini` ou `my.cnf`
- Verificar se a porta 3306 está configurada
- Verificar se o bind-address está correto

---

## 🧪 **Testar Conexão**

### **1. Via MySQL Workbench**
- Conectar com as credenciais configuradas
- Verificar se consegue acessar os bancos

### **2. Via linha de comando**
```bash
mysql -u root -p15Dev -h localhost -P 3306
```

### **3. Via aplicação**
```bash
cd src/AvenSuites-Api
dotnet run
```

---

## 📝 **Configurações Alternativas**

### **Se usar porta diferente**
```json
"DefaultConnection": "Server=localhost;Database=AvenSuitesDb;Uid=root;Pwd=15Dev;Port=3307;"
```

### **Se usar usuário diferente**
```json
"DefaultConnection": "Server=localhost;Database=AvenSuitesDb;Uid=seuusuario;Pwd=suasenha;Port=3306;"
```

### **Se usar servidor remoto**
```json
"DefaultConnection": "Server=192.168.1.100;Database=AvenSuitesDb;Uid=root;Pwd=15Dev;Port=3306;"
```

---

## ✅ **Verificação Final**

Após configurar, execute:

```bash
cd src/AvenSuites-Api
dotnet run
```

Se tudo estiver correto, você deve ver:
- ✅ Compilação bem-sucedida
- ✅ Conexão com banco estabelecida
- ✅ Tabelas criadas automaticamente
- ✅ API rodando na porta configurada

---

## 🆘 **Se ainda houver problemas**

1. **Verifique os logs** da aplicação para mais detalhes
2. **Teste a conexão** diretamente no MySQL Workbench
3. **Verifique as credenciais** do usuário root
4. **Confirme se o MariaDB** está rodando na porta 3306
5. **Verifique firewall** se necessário

