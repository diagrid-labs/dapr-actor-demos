# Use an official Node.js runtime as a parent image
FROM node:20-alpine3.18

# Set the working directory in the container to /app
WORKDIR /front-end

# Copy package.json and package-lock.json into the container at /app
COPY package*.json ./

# Install any needed packages specified in package.json
RUN npm install

# Copy the rest of your app's source code into the container at /app
COPY . .

# Make port 5500 available to the world outside this container
EXPOSE 5500

# Run your app when the container launches
CMD ["npm", "start"]