# 1. Usamos la imagen oficial de Tomcat con JDK 21
FROM tomcat:10.1-jdk21-openjdk-slim

# 2. Definimos el directorio de despliegue de Tomcat
WORKDIR /usr/local/tomcat/webapps/

# 3. Borramos las apps por defecto para evitar conflictos (opcional)
RUN rm -rf ROOT

# 4. Copiamos el archivo WAR que exportaste desde Eclipse
# Asegúrate de que el nombre coincida con el que generaste en la carpeta target
COPY target/VotoSeguro.war ./ROOT.war

# 5. Exponemos el puerto 8080
EXPOSE 8080

# 6. Ejecutamos Tomcat
CMD ["catalina.sh", "run"]