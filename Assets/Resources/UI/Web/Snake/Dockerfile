FROM nginx:1.27-alpine

# Serve the snake game on its own port
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY Snake.html /usr/share/nginx/html/index.html

EXPOSE 1002

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD wget -qO- http://127.0.0.1:1002/ >/dev/null || exit 1
