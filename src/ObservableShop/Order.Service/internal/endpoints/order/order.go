package endpoints

import (
	"context"
	"encoding/json"
	"fmt"
	"math/rand"
	"net/http"
	"time"

	database "example/order-service/internal/dataaccess"
	types "example/order-service/internal/models"

	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/attribute"
	"go.opentelemetry.io/otel/metric"
)

var (
	order_tracer = otel.Tracer("order")
	order_meter  = otel.Meter("order")
	order_Count  metric.Int64Counter
)

func init() {
	var err error
	order_Count, err = order_meter.Int64Counter("order.count",
		metric.WithDescription("The number of orders"))
	if err != nil {
		panic(err)
	}
}

func NewOrder(ctx context.Context) types.Order {
	// You can use a prefix or timestamp for the orderId
	prefix := "OR"
	timestamp := time.Now().Unix()

	// Generate a random number for the orderId
	randomNumber := rand.Intn(10000)

	// Combine prefix, timestamp, and random number to create orderId
	orderId := fmt.Sprintf("%s%d%d", prefix, timestamp, randomNumber)

	order := types.Order{ID: orderId}

	database.AddOrder(order, ctx)

	return order
}

func Order(w http.ResponseWriter, r *http.Request) {
	ctx, span := order_tracer.Start(r.Context(), "order")
	defer span.End()

	if r.Method != http.MethodPost {
		http.Error(w, "Method Not Allowed", http.StatusMethodNotAllowed)
		return
	}

	order := NewOrder(ctx)

	// Add the custom attribute to the span and counter.
	orderIdAttribute := attribute.String("order.id", order.ID)
	span.SetAttributes(orderIdAttribute)
	order_Count.Add(ctx, 1, metric.WithAttributes(orderIdAttribute))

	// Marshal the Order struct into a JSON string
	orderJSON, err := json.Marshal(order)
	if err != nil {
		// Handle the error if JSON marshaling fails
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	// Set the Content-Type header to indicate that the response body is JSON
	w.Header().Set("Content-Type", "application/json")

	// Set the HTTP status code to OK (200) and write the JSON response
	w.WriteHeader(http.StatusOK)
	w.Write(orderJSON)
}
