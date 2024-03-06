FROM python:3.12-slim as base

RUN apt-get update && apt-get install

RUN apt-get install -y \
  dos2unix \
  libpq-dev \
  libmariadb-dev-compat \
  libmariadb-dev \
  gcc \
  && apt-get clean

RUN python -m pip install --upgrade pip

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy the current directory contents into the container at /usr/src/app
COPY . .

# Install any needed packages specified in requirements.txt
RUN pip install psycopg2
RUN pip install --no-cache-dir -r requirements.txt
# #install opentelemetry packages
# # RUN pip install opentelemetry-api \
# #     install opentelemetry-sdk
# RUN pip install opentelemetry-distro \
# 	opentelemetry-exporter-otlp

# RUN opentelemetry-bootstrap -a install



EXPOSE 50051
CMD ["python", "product.api/server.py"]
