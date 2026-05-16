# ==========================================
# ETAPA 1 - BUILD CON MAVEN
# ==========================================
FROM maven:3.9.6-eclipse-temurin-21 AS build

WORKDIR /app

# Copiamos pom.xml
COPY pom.xml .

# Copiamos código fuente
COPY src ./src

# Compilamos y generamos WAR
RUN mvn clean package -DskipTests

# ==========================================
# ETAPA 2 - TOMCAT
# ==========================================
FROM tomcat:10.1-jdk21-openjdk-slim

WORKDIR /usr/local/tomcat/webapps/

# Eliminamos aplicaciones por defecto
RUN rm -rf ROOT *

# Copiamos el WAR generado
COPY --from=build /app/target/*.war ROOT.war

# Puerto de Tomcat
EXPOSE 8080

# Ejecutar Tomcat
CMD ["catalina.sh", "run"]