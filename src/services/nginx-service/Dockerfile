################################
#### PRODUCTION ENVIRONMENT ####
################################

# Use the official NGINX image for production
FROM nginx AS production

# copy nginx configuration in side conf.d folder
COPY ./default.conf /etc/nginx/conf.d/default.conf

# Expose port 80 to allow access to the app
EXPOSE 80

# Run Nginx in the foreground
ENTRYPOINT ["nginx", "-g", "daemon off;"]