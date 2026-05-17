package websocket;

import jakarta.websocket.server.ServerEndpointConfig;
import jakarta.websocket.server.ServerEndpointConfig.Configurator;
import jakarta.servlet.ServletContextEvent;
import jakarta.servlet.ServletContextListener;
import jakarta.servlet.annotation.WebListener;
import org.apache.tomcat.websocket.server.WsSci;

@WebListener
public class WebSocketInitializer implements ServletContextListener {

    @Override
    public void contextInitialized(ServletContextEvent sce) {
        // Inicializa WebSocket en Tomcat
    }

    @Override
    public void contextDestroyed(ServletContextEvent sce) {
    }
}