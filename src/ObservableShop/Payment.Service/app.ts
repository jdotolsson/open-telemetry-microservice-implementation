/*app.ts*/
import express, { Express } from 'express';
import * as apiLogs from "@opentelemetry/api-logs";

const PORT: number = parseInt(process.env.PORT || '8080');
const app: Express = express();

const logger = apiLogs.logs.getLogger("default");
app.post('/payment', (req, res) => {

  logger.emit({
    severityNumber: apiLogs.SeverityNumber.ERROR,
    severityText: 'ERROR',
    body: 'Payment request received'
  });
  res.status(201).send();
});

app.listen(PORT, () => {
  console.log(`Listening for requests on http://localhost:${PORT}`);
});
