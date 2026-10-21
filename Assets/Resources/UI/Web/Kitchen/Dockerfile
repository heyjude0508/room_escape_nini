FROM nginx:1.27-alpine

# Serve the static site
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY Kitchen.html /usr/share/nginx/html/index.html

EXPOSE 1001

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD wget -qO- http://127.0.0.1:1001/ >/dev/null || exit 1
