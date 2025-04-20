# Retry Pattern

## **What is the Retry Pattern?**

![Alt text](../images/retry.png)

The Retry Pattern is a resilience design pattern that automatically re-attempts a failed operation, typically after a short delay, with the expectation that the operation will succeed on a subsequent try. It is especially useful for handling transient faults—temporary issues such as network timeouts, service unavailability, or intermittent errors—that often resolve themselves if retried.

---

## **Parameters of the Retry Pattern**

- **Max Retry Attempts:** Maximum number of times to retry the operation before giving up.
- **Delay Between Retries:** Time to wait between each retry attempt. This can be a fixed interval or use an exponential backoff strategy.
- **Backoff Strategy:** How the delay increases between retries (e.g., fixed, linear, exponential).
- **Retryable Exceptions/Conditions:** Specific errors or status codes that should trigger a retry.
- **Timeout:** Maximum total time to keep retrying before aborting.
- **Jitter:** Randomized delay added to avoid retry storms in distributed systems.

---

## **Flow of the Retry Pattern**

1. **Attempt Operation:** Try to execute the operation.
2. **Check for Failure:** If the operation fails with a retryable error, proceed to retry logic.
3. **Wait:** Pause for the configured delay (using backoff strategy if applicable).
4. **Retry:** Attempt the operation again.
5. **Repeat:** Continue steps 2–4 until the operation succeeds or the maximum number of retries is reached.
6. **Handle Exhaustion:** If all retries fail, log the error or escalate.

---

## **How Polly Implements Retry (in .NET)**

[Polly](https://github.com/App-vNext/Polly) is a popular .NET library for resilience and transient-fault handling, providing robust support for the Retry pattern.

- **Configuration:** You specify how many times to retry, which exceptions to handle, and the delay/backoff strategy.
- **Usage Example:**
  ```
  var retryPolicy = Policy
      .Handle<Exception>()
      .WaitAndRetry(
          retryCount: 3,
          sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)) // Exponential backoff
      );
  ```
- **Integration:** Polly can combine retries with other policies like circuit breaker, fallback, and timeout.
- **Advanced Features:** Supports on-retry callbacks, jitter, and context passing.

---

## **When to Use the Retry Pattern**

- When interacting with **remote services, APIs, or databases** that may experience transient failures.
- In **distributed systems** where network or service interruptions are common.
- For **operations that are idempotent** (safe to repeat without side effects).

---

## **When Not to Use the Retry Pattern**

- For **non-transient or persistent failures** (e.g., authentication errors, invalid input).
- On **non-idempotent operations** where repeating the operation could cause unintended side effects.
- If retrying could **overwhelm a failing service** or lead to cascading retry storms.
- For **local operations** unlikely to benefit from retries.

---

## **Benefits**

- **Improved fault tolerance:** Handles transient errors gracefully, increasing system reliability[.
- **Better user experience:** Reduces visible errors and disruptions.
- **Simplicity:** Encapsulates retry logic for reuse and easier maintenance.

---

## **Drawbacks**

- **Increased latency:** Retries can delay failure responses, especially with multiple attempts.
- **Resource consumption:** Multiple retries can consume more resources and bandwidth.
- **Risk of retry storms:** Without proper coordination, simultaneous retries can overload dependencies.
- **Not suitable for all failures:** Ineffective for persistent or logic errors.

---

## **Retry Pattern vs. Circuit Breaker Pattern**

| Aspect                    | Retry Pattern                                                 | Circuit Breaker Pattern                                          |
| ------------------------- | ------------------------------------------------------------- | ---------------------------------------------------------------- |
| Purpose                   | Retries failed operations to handle transient faults          | Prevents repeated calls to a failing service                     |
| Response to Failures      | Retries failed operations automatically                       | Blocks requests after a failure threshold is exceeded            |
| State Management          | Stateless; each retry is independent                          | Maintains state (Closed, Open, Half-Open)                        |
| Fault Detection           | Detects and retries individual failures                       | Monitors overall service health and adapts behavior              |
| Latency Impact            | Can increase latency due to repeated retries                  | May block requests, introducing latency during state transitions |
| Handling Transient Faults | Attempts to resolve by retrying                               | Blocks requests, allowing service to recover                     |
| Use Together?             | Yes—retry first, then use circuit breaker to prevent overload | Yes—combine for robust resilience                                |

---

## **How Retry and Circuit Breaker Work Together**

- **Combined Usage:** The Retry pattern can be used for transient, recoverable errors. If failures persist and reach a threshold, the Circuit Breaker pattern trips to prevent further retries, protecting the system from overload[3].
- **Typical Flow:** Retry attempts are made for each failure. If the circuit breaker is open, retries are bypassed, and fallback logic is triggered instead.
- **Best Practice:** Use retries for short-lived issues and circuit breakers to guard against persistent or systemic failures.

---

## **Summary**

The Retry Pattern is a simple and effective way to handle transient failures by automatically retrying operations, improving resilience and user experience. It should be used thoughtfully, especially in combination with the Circuit Breaker pattern, to avoid overwhelming failing services and to provide robust fault tolerance in distributed systems.
